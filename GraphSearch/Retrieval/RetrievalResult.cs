namespace GraphSearch.Library.Retrieval
{
    public sealed record RetrievalResult(
    long NodeId,
    double VectorScore,
    double Bm25Score,
    double RrfScore,
    double GraphScore,
    double FinalScore);
}
