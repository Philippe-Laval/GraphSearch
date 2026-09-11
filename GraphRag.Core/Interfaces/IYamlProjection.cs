namespace GraphRag.Core.Interfaces;

public interface IYamlProjection<in T>
{
    string Project(T value);
}