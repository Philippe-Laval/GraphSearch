namespace GraphRag.Ontology.Itsm.Business;

/// <summary>
/// Base class for all ticket kinds. 
/// Concrete types are Incident, ServiceRequest, Problem, ChangeRequest, Task.
/// </summary>
public abstract class Ticket : WorkItem;
