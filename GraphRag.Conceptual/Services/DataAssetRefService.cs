using GraphRag.Conceptual.Context;
using GraphRag.Conceptual.Entities;

namespace GraphRag.Conceptual.Services;

public class DataAssetRefService : CrudService<DataAssetRef>
{
    public DataAssetRefService(OntologyDbContext dbContext) : base(dbContext) { }
}
