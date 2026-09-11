using GraphRag.Core.Models;

namespace GraphRag.Core.Interfaces;

public interface IProjection<T>
{
    EmbeddingDocument<T> Project(T value);
}