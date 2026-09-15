using GraphRag.Conceptual.Context;
using GraphRag.Conceptual.Entities;

namespace GraphRag.Conceptual.Services;

public class CompetencyQuestionService : CrudService<CompetencyQuestion>
{
    public CompetencyQuestionService(OntologyDbContext dbContext) : base(dbContext) { }
}
