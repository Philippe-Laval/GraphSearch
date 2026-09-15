using GraphRag.Conceptual.Context;
using GraphRag.Conceptual.Entities;

namespace GraphRag.Conceptual.Services;

public class ConceptRepresentationService : CrudService<ConceptRepresentation>
{
    public ConceptRepresentationService(OntologyDbContext dbContext) : base(dbContext) { }
}
