using GraphRag.ITSM.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GraphRag.ITSM.Extensions;

/// <summary>
/// DI registration for all ITSM CRUD services declared in <c>GraphRag.ITSM.Services</c>.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddItsmServices(this IServiceCollection services)
    {
        // Party
        services.AddScoped<PersonService>();
        services.AddScoped<AgentService>();
        services.AddScoped<OrganizationService>();
        services.AddScoped<AssignmentGroupService>();
        services.AddScoped<VendorService>();
        services.AddScoped<AgentGroupMembershipService>();

        // Services / Catalog
        services.AddScoped<BusinessServiceService>();
        services.AddScoped<TechnicalServiceService>();
        services.AddScoped<ServiceOfferingService>();
        services.AddScoped<ServiceCatalogItemService>();
        services.AddScoped<ServiceSupportGroupService>();
        services.AddScoped<TechnicalServiceEnablementService>();
        services.AddScoped<BusinessServiceConsumerService>();

        // Configuration Items
        services.AddScoped<ApplicationService>();
        services.AddScoped<ServerService>();
        services.AddScoped<DatabaseCIService>();
        services.AddScoped<NetworkDeviceService>();
        services.AddScoped<CloudResourceService>();
        services.AddScoped<ConfigurationItemSupportService>();
        services.AddScoped<ConfigurationItemDependencyService>();

        // Classification
        services.AddScoped<CategoryService>();
        services.AddScoped<StatusService>();
        services.AddScoped<ImpactService>();
        services.AddScoped<UrgencyService>();
        services.AddScoped<PriorityService>();
        services.AddScoped<SeverityService>();

        // Tickets
        services.AddScoped<IncidentService>();
        services.AddScoped<ServiceRequestService>();
        services.AddScoped<ProblemService>();
        services.AddScoped<ChangeRequestService>();
        services.AddScoped<TicketTaskService>();
        services.AddScoped<TicketAffectedCIService>();
        services.AddScoped<TicketAffectedServiceService>();
        services.AddScoped<TicketRelationService>();

        // Operational Events
        services.AddScoped<StatusTransitionService>();
        services.AddScoped<AssignmentEventService>();
        services.AddScoped<EscalationEventService>();
        services.AddScoped<ReassignmentEventService>();
        services.AddScoped<ResolutionEventService>();
        services.AddScoped<ReopenEventService>();
        services.AddScoped<ClosureEventService>();

        // SLA
        services.AddScoped<ServiceLevelAgreementService>();
        services.AddScoped<SlaMeasurementService>();
        services.AddScoped<BusinessCalendarService>();
        services.AddScoped<CalendarHolidayService>();

        return services;
    }
}
