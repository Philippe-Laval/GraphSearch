using GraphRag.Conceptual.Context;
using GraphRag.Conceptual.Entities;

namespace GraphRag.Conceptual.Services;

public class AccessPolicyRuleService : CrudService<AccessPolicyRule>
{
    public AccessPolicyRuleService(OntologyDbContext dbContext) : base(dbContext) { }
}
