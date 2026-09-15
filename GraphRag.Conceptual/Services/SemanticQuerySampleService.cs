using GraphRag.Conceptual.Context;
using GraphRag.Conceptual.Entities;

namespace GraphRag.Conceptual.Services;

public class SemanticQuerySampleService : CrudService<SemanticQuerySample>
{
    public SemanticQuerySampleService(OntologyDbContext dbContext) : base(dbContext) { }
}
