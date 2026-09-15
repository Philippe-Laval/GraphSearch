using GraphRag.ITSM.Context;
using GraphRag.ITSM.Entities;
using Microsoft.EntityFrameworkCore;

namespace GraphRag.ITSM.Services;

public class PersonService : EntityService<Person>
{
    public PersonService(ItsmDbContext dbContext) : base(dbContext) { }
}

public class AgentService : EntityService<Agent>
{
    public AgentService(ItsmDbContext dbContext) : base(dbContext) { }
}

public class OrganizationService : EntityService<Organization>
{
    public OrganizationService(ItsmDbContext dbContext) : base(dbContext) { }
}

public class AssignmentGroupService : EntityService<AssignmentGroup>
{
    public AssignmentGroupService(ItsmDbContext dbContext) : base(dbContext) { }
}

/// <summary>Join entity Agent &lt;-&gt; AssignmentGroup (composite key).</summary>
public class AgentGroupMembershipService
{
    private readonly ItsmDbContext _dbContext;

    public AgentGroupMembershipService(ItsmDbContext dbContext) => _dbContext = dbContext;

    public async Task<IReadOnlyList<AgentGroupMembership>> GetAllAsync(CancellationToken ct = default)
        => await _dbContext.AgentGroupMemberships.AsNoTracking().ToListAsync(ct);

    public async Task<AgentGroupMembership?> GetAsync(Guid agentId, Guid assignmentGroupId, CancellationToken ct = default)
        => await _dbContext.AgentGroupMemberships.AsNoTracking()
            .FirstOrDefaultAsync(m => m.AgentId == agentId && m.AssignmentGroupId == assignmentGroupId, ct);

    public async Task<AgentGroupMembership> CreateAsync(AgentGroupMembership membership, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(membership);
        if (membership.AssignedAt == default)
        {
            membership.AssignedAt = DateTimeOffset.UtcNow;
        }
        await _dbContext.AgentGroupMemberships.AddAsync(membership, ct);
        await _dbContext.SaveChangesAsync(ct);
        return membership;
    }

    public async Task<AgentGroupMembership?> UpdateAsync(AgentGroupMembership membership, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(membership);
        var existing = await _dbContext.AgentGroupMemberships
            .FirstOrDefaultAsync(m => m.AgentId == membership.AgentId && m.AssignmentGroupId == membership.AssignmentGroupId, ct);
        if (existing is null) return null;

        existing.AssignedAt = membership.AssignedAt;
        existing.RemovedAt = membership.RemovedAt;
        existing.RoleInGroup = membership.RoleInGroup;

        await _dbContext.SaveChangesAsync(ct);
        return existing;
    }

    public async Task<bool> DeleteAsync(Guid agentId, Guid assignmentGroupId, CancellationToken ct = default)
    {
        var existing = await _dbContext.AgentGroupMemberships
            .FirstOrDefaultAsync(m => m.AgentId == agentId && m.AssignmentGroupId == assignmentGroupId, ct);
        if (existing is null) return false;
        _dbContext.AgentGroupMemberships.Remove(existing);
        await _dbContext.SaveChangesAsync(ct);
        return true;
    }
}
