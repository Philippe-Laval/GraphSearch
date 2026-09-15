using GraphRag.Conceptual.Context;
using GraphRag.Conceptual.Entities;

namespace GraphRag.Conceptual.Services;

public class BusinessMetricDefinitionService : CrudService<BusinessMetricDefinition>
{
    public BusinessMetricDefinitionService(OntologyDbContext dbContext) : base(dbContext) { }
}
