using GraphRag.Conceptual.Context;
using GraphRag.Conceptual.Entities;

namespace GraphRag.Conceptual.Services;

public class DataOwnerRecordService : CrudService<DataOwnerRecord>
{
    public DataOwnerRecordService(OntologyDbContext dbContext) : base(dbContext) { }
}
