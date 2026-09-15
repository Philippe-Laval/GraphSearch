using GraphRag.ITSM.Context;
using GraphRag.ITSM.Entities;

namespace GraphRag.ITSM.Services;

public class StatusTransitionService : EntityService<StatusTransition>
{
    public StatusTransitionService(ItsmDbContext dbContext) : base(dbContext) { }
}

public class AssignmentEventService : EntityService<AssignmentEvent>
{
    public AssignmentEventService(ItsmDbContext dbContext) : base(dbContext) { }
}

public class EscalationEventService : EntityService<EscalationEvent>
{
    public EscalationEventService(ItsmDbContext dbContext) : base(dbContext) { }
}

public class ReassignmentEventService : EntityService<ReassignmentEvent>
{
    public ReassignmentEventService(ItsmDbContext dbContext) : base(dbContext) { }
}

public class ResolutionEventService : EntityService<ResolutionEvent>
{
    public ResolutionEventService(ItsmDbContext dbContext) : base(dbContext) { }
}

public class ReopenEventService : EntityService<ReopenEvent>
{
    public ReopenEventService(ItsmDbContext dbContext) : base(dbContext) { }
}

public class ClosureEventService : EntityService<ClosureEvent>
{
    public ClosureEventService(ItsmDbContext dbContext) : base(dbContext) { }
}
