using GraphRag.Conceptual.Context;
using GraphRag.Conceptual.Entities;

namespace GraphRag.Conceptual.Services;

public class QualityConstraintService : CrudService<QualityConstraint>
{
    public QualityConstraintService(OntologyDbContext dbContext) : base(dbContext) { }
}
