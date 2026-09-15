using GraphRag.Conceptual.Context;
using GraphRag.Conceptual.Entities;

namespace GraphRag.Conceptual.Services;

public class DataClassificationRuleService : CrudService<DataClassificationRule>
{
    public DataClassificationRuleService(OntologyDbContext dbContext) : base(dbContext) { }
}
