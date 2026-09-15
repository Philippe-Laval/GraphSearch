using GraphRag.Conceptual.Context;
using GraphRag.Conceptual.Entities;

namespace GraphRag.Conceptual.Services;

public class ConceptService : CrudService<Concept>
{
    public ConceptService(OntologyDbContext dbContext) : base(dbContext) { }
}
