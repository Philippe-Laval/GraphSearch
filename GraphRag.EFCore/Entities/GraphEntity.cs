namespace GraphRag.EFCore.Entities;

public class GraphEntity
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string? Description { get; set; }

    public ICollection<GraphEntityAlias> Aliases { get; set; } = new List<GraphEntityAlias>();
}
