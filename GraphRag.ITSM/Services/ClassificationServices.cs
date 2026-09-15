using GraphRag.ITSM.Context;
using GraphRag.ITSM.Entities;

namespace GraphRag.ITSM.Services;

public class CategoryService : EntityService<Category>
{
    public CategoryService(ItsmDbContext dbContext) : base(dbContext) { }
}

public class StatusService : EntityService<Status>
{
    public StatusService(ItsmDbContext dbContext) : base(dbContext) { }
}

public class ImpactService : EntityService<Impact>
{
    public ImpactService(ItsmDbContext dbContext) : base(dbContext) { }
}

public class UrgencyService : EntityService<Urgency>
{
    public UrgencyService(ItsmDbContext dbContext) : base(dbContext) { }
}

public class PriorityService : EntityService<Priority>
{
    public PriorityService(ItsmDbContext dbContext) : base(dbContext) { }
}

public class SeverityService : EntityService<Severity>
{
    public SeverityService(ItsmDbContext dbContext) : base(dbContext) { }
}
