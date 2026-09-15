using GraphRag.Conceptual.Context;
using GraphRag.Conceptual.Entities;

namespace GraphRag.Conceptual.Services;

public class TermService : CrudService<Term>
{
    public TermService(OntologyDbContext dbContext) : base(dbContext) { }
}
