namespace GraphRag.Ontology.Itsm.Business;

public sealed class TicketTask : Ticket
{
    public Ticket? ParentTicket { get; init; }
}
