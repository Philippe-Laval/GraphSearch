namespace GraphRag.EFCore.Entities;

public class GraphEntityAlias
{
    public int EntityId { get; set; }

    public string Alias { get; set; } = null!;

    public GraphEntity Entity { get; set; } = null!;
}
