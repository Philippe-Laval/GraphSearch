namespace GraphSearch.Library.Query.Analysis.EntityExtraction;

/// <summary>
/// Aho–Corasick multi-pattern matcher over an entity gazetteer.
///
/// <para>
/// Where the naive <see cref="DictionaryEntityExtractor"/> is O(entities × n)
/// (one <c>IndexOf</c> pass per entity name), Aho–Corasick is O(n + matches)
/// after an O(Σ|patterns|) one-time build. Worth switching to once the
/// vocabulary exceeds a few thousand entries.
/// </para>
///
/// <para>
/// Matching is case-insensitive (patterns are lowercased at build time and the
/// query is lowercased at scan time). The <see cref="ExtractedEntity.Text"/>
/// value returned preserves the original casing from the query. Overlapping
/// hits are resolved by <see cref="EntitySpanMerger"/>: the longest, highest
/// confidence span wins.
/// </para>
/// </summary>
public sealed class AhoCorasickGazetteerExtractor : IEntityExtractor
{
    private readonly AhoCorasickAutomaton _automaton;
    private readonly double _confidence;

    public AhoCorasickGazetteerExtractor(
        IEnumerable<EntityDefinition> entities,
        double confidence = 1.0)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _confidence = confidence;
        _automaton = AhoCorasickAutomaton.Build(entities);
    }

    public Task<IReadOnlyList<ExtractedEntity>> ExtractAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);
        cancellationToken.ThrowIfCancellationRequested();

        var accumulator = new List<(ExtractedEntity Entity, int Priority)>();

        foreach (var hit in _automaton.FindAll(query))
        {
            var entity = new ExtractedEntity(
                Text: query.Substring(hit.Start, hit.Length),
                Type: hit.Definition.Type,
                Start: hit.Start,
                Length: hit.Length,
                Confidence: _confidence);

            EntitySpanMerger.AddOrReplace(accumulator, entity, priority: 0);
        }

        return Task.FromResult(EntitySpanMerger.Finalize(accumulator));
    }

    // Compact Aho–Corasick implementation. Nodes are stored in arrays; goto
    // transitions use a per-node dictionary (sparse). Sufficient for typical
    // gazetteer sizes (<1M patterns) and avoids managed pointer chasing.
    private sealed class AhoCorasickAutomaton
    {
        private readonly List<Dictionary<char, int>> _goto = new();
        private readonly List<int> _fail = new();
        private readonly List<List<EntityDefinition>?> _output = new();

        private AhoCorasickAutomaton()
        {
            AddNode();
        }

        public static AhoCorasickAutomaton Build(IEnumerable<EntityDefinition> entities)
        {
            var a = new AhoCorasickAutomaton();

            foreach (var e in entities)
            {
                if (string.IsNullOrEmpty(e.Name))
                {
                    continue;
                }
                a.AddPattern(e);
            }

            a.BuildFailureLinks();
            return a;
        }

        private int AddNode()
        {
            _goto.Add(new Dictionary<char, int>());
            _fail.Add(0);
            _output.Add(null);
            return _goto.Count - 1;
        }

        private void AddPattern(EntityDefinition definition)
        {
            var node = 0;
            var pattern = definition.Name;
            for (var i = 0; i < pattern.Length; i++)
            {
                var c = char.ToLowerInvariant(pattern[i]);
                if (!_goto[node].TryGetValue(c, out var next))
                {
                    next = AddNode();
                    _goto[node][c] = next;
                }
                node = next;
            }
            (_output[node] ??= new List<EntityDefinition>()).Add(definition);
        }

        private void BuildFailureLinks()
        {
            var queue = new Queue<int>();
            foreach (var (_, child) in _goto[0])
            {
                _fail[child] = 0;
                queue.Enqueue(child);
            }

            while (queue.Count > 0)
            {
                var r = queue.Dequeue();
                foreach (var (c, u) in _goto[r])
                {
                    queue.Enqueue(u);
                    var state = _fail[r];
                    while (state != 0 && !_goto[state].ContainsKey(c))
                    {
                        state = _fail[state];
                    }
                    _fail[u] = _goto[state].TryGetValue(c, out var f) && f != u ? f : 0;

                    if (_output[_fail[u]] is { } inherited)
                    {
                        (_output[u] ??= new List<EntityDefinition>()).AddRange(inherited);
                    }
                }
            }
        }

        public IEnumerable<(int Start, int Length, EntityDefinition Definition)> FindAll(string text)
        {
            var state = 0;
            for (var i = 0; i < text.Length; i++)
            {
                var c = char.ToLowerInvariant(text[i]);
                while (state != 0 && !_goto[state].ContainsKey(c))
                {
                    state = _fail[state];
                }
                if (_goto[state].TryGetValue(c, out var next))
                {
                    state = next;
                }

                if (_output[state] is { } hits)
                {
                    foreach (var def in hits)
                    {
                        var length = def.Name.Length;
                        var start = i - length + 1;
                        if (start >= 0)
                        {
                            yield return (start, length, def);
                        }
                    }
                }
            }
        }
    }
}
