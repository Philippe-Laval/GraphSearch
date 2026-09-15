using GraphRag.Conceptual.Context;
using GraphRag.Conceptual.Entities;

namespace GraphRag.Conceptual.Services;

public class JoinRelationshipService : CrudService<JoinRelationship>
{
    public JoinRelationshipService(OntologyDbContext dbContext) : base(dbContext) { }
}
