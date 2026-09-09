namespace GraphSearch.Library.Query.Analysis.Rewriting;

/// <summary>
/// Classification of a <see cref="QueryRewrite"/>. Consumers can use it to
/// route rewrites to different retrievers (e.g. send <see cref="HyDE"/> to the
/// vector index only) or to weight results during fusion.
/// </summary>
public enum RewriteKind
{
    /// <summary>Unknown / unclassified rewrite.</summary>
    Unknown = 0,

    /// <summary>Dictionary or lexical synonym substitution.</summary>
    Synonym,

    /// <summary>Regex/template-based rewrite (e.g. "who made X" → "who developed X").</summary>
    Template,

    /// <summary>Morphological variant (stem/lemma) of the original query.</summary>
    Morphological,

    /// <summary>Entity mention replaced with a knowledge-graph alias.</summary>
    EntityAlias,

    /// <summary>Free-form paraphrase produced by an LLM.</summary>
    LlmParaphrase,

    /// <summary>Hypothetical Document Embedding paragraph — for vector retrieval only.</summary>
    HyDE,

    /// <summary>Sub-question produced by query decomposition.</summary>
    SubQuery,

    /// <summary>Re-verbalisation of a parsed graph query pattern.</summary>
    GraphSchema,

    /// <summary>Back-translation paraphrase.</summary>
    BackTranslation,
}
