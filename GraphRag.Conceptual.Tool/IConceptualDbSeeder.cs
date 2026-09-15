namespace GraphRag.Conceptual.Tool
{
    public interface IConceptualDbSeeder
    {
        Task Seed(CancellationToken token = default);
    }
}