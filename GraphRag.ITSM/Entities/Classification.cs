namespace GraphRag.ITSM.Entities;

/// <summary>Common shape for classification lookup values.</summary>
public abstract class ClassificationLookup : EntityBase
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
}

public class Category : ClassificationLookup
{
    public Guid? ParentCategoryId { get; set; }
    public Category? ParentCategory { get; set; }
    public ICollection<Category> Children { get; set; } = [];
}

public enum StatusKind
{
    New,
    Acknowledged,
    Assigned,
    InProgress,
    Pending,
    Resolved,
    Closed,
    Cancelled,
    Reopened,
}

public class Status : ClassificationLookup
{
    public StatusKind Kind { get; set; }
    public bool IsTerminal { get; set; }
}

public enum ImpactLevel { Low, Medium, High }
public enum UrgencyLevel { Low, Medium, High }
public enum PriorityLevel { P1, P2, P3, P4, P5 }
public enum SeverityLevel { Critical, High, Medium, Low, Informational }

public class Impact : ClassificationLookup
{
    public ImpactLevel Level { get; set; }
}

public class Urgency : ClassificationLookup
{
    public UrgencyLevel Level { get; set; }
}

public class Priority : ClassificationLookup
{
    public PriorityLevel Level { get; set; }
    public Guid? DerivedFromImpactId { get; set; }
    public Impact? DerivedFromImpact { get; set; }
    public Guid? DerivedFromUrgencyId { get; set; }
    public Urgency? DerivedFromUrgency { get; set; }
}

public class Severity : ClassificationLookup
{
    public SeverityLevel Level { get; set; }
}
