namespace GraphRag.Ontology.Itsm.Business;

/// <summary>
/// Task
/// </summary>
public sealed class TicketTask : Ticket
{
    public Ticket? ParentTicket { get; init; }
}
