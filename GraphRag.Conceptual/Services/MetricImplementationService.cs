using GraphRag.Conceptual.Context;
using GraphRag.Conceptual.Entities;

namespace GraphRag.Conceptual.Services;

public class MetricImplementationService : CrudService<MetricImplementation>
{
    public MetricImplementationService(OntologyDbContext dbContext) : base(dbContext) { }
}
