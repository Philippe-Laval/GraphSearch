namespace GraphRag.Core.Interfaces;

public interface ITextGenerator<in T>
{
    Task<string> GenerateAsync(T item, CancellationToken cancellationToken = default);
}
