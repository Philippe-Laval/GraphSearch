namespace GraphRag.ITSM.Tool
{
    public interface IItsmDbSeeder
    {
        Task Seed(CancellationToken token = default);
    }
}