namespace GraphSearch.Library.Query.Analysis;

/// <summary>
/// Char span in the original source string.
/// </summary>
public readonly record struct CharSpan(int Start, int Length)
{
    public int End => Start + Length;
    public static readonly CharSpan None = new(-1, 0);
    public bool IsSpecial => Start < 0 || Length == 0;
}

/// <summary>
/// Model-ready encoding of a query for a HuggingFace token-classification model.
/// </summary>
/// <param name="InputIds">Token ids (int64).</param>
/// <param name="AttentionMask">1 for real tokens, 0 for padding.</param>
/// <param name="TokenTypeIds">Optional (BERT). CamemBERT etc. don't use this.</param>
/// <param name="TokenCharSpans">
/// For each token, its char span in the original text.
/// Use <see cref="CharSpan.None"/> for special/padding tokens ([CLS], [SEP], [PAD]).
/// </param>
public sealed record HuggingFaceEncoding(
    long[] InputIds,
    long[] AttentionMask,
    long[]? TokenTypeIds,
    IReadOnlyList<CharSpan> TokenCharSpans);

/// <summary>
/// Minimal tokenizer abstraction used by <see cref="HuggingFaceOnnxNerService"/>.
///
/// Implement this against your tokenizer of choice — the recommended options are:
/// <list type="bullet">
///   <item><b>Microsoft.ML.Tokenizers</b> (BertTokenizer / SentencePieceTokenizer). Use for BERT-style models.</item>
///   <item><b>FastTokenizer</b> ports of HuggingFace's Rust tokenizers.</item>
///   <item>A thin P/Invoke wrapper over the Python tokenizer if you already run one server-side.</item>
/// </list>
///
/// The abstraction is intentional: the C# NER pipeline is stable even as the
/// tokenizer libraries evolve.
/// </summary>
public interface IHuggingFaceTokenizer
{
    HuggingFaceEncoding Encode(string text);
}
