using GraphRag.Ontology.Itsm.Core;

namespace GraphRag.Ontology.Itsm.Business.Party;

/// <summary>Any human or organizational actor involved in ITSM operations.</summary>
public abstract class Party : BusinessConcept
{
    public Organization? Organization { get; init; }
}

/// <summary>End-user of a service (may or may not raise tickets themselves).</summary>
public class User : Party
{
    public string? Email { get; init; }
    public string? Department { get; init; }
    public string? Location { get; init; }
}

/// <summary>User who submitted a ticket. Semantically distinct from AffectedUser.</summary>
public sealed class Requester : User;

/// <summary>Support employee working on tickets.</summary>
public class Agent : User
{
    public IReadOnlyList<AssignmentGroup> Groups { get; init; } = [];
}

/// <summary>A team that owns tickets (aka support group / resolution group).</summary>
public class AssignmentGroup : Party
{
    public string? Department { get; init; }
    public IReadOnlyList<Service.Service> SupportedServices { get; init; } = [];
}

/// <summary>Specialization of AssignmentGroup used specifically to denote L1/L2/L3 support teams.</summary>
public sealed class SupportTeam : AssignmentGroup
{
    public string? SupportTier { get; init; }
}

public sealed class Vendor : Party
{
    public string? ContractReference { get; init; }
}

public class Organization : Party
{
    public string? BusinessUnit { get; init; }
    public Organization? Parent { get; init; }
}
