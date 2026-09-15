using GraphRag.Conceptual.Context;
using GraphRag.Conceptual.Entities;

namespace GraphRag.Conceptual.Services;

public class ConceptRelationService : CrudService<ConceptRelation>
{
    public ConceptRelationService(OntologyDbContext dbContext) : base(dbContext) { }
}
