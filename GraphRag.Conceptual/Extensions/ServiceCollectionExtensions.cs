using GraphRag.Conceptual.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GraphRag.Conceptual.Extensions;

/// <summary>
/// DI registration for all Conceptual CRUD services declared in <c>GraphRag.Conceptual.Services</c>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Scans the <c>GraphRag.Conceptual.Services</c> namespace for every concrete class
    /// deriving from <see cref="CrudService{T}"/> and registers it as a scoped service.
    /// Each service is also exposed through its <see cref="ICrudService{T}"/> contract.
    /// </summary>
    public static IServiceCollection AddConceptualServices(this IServiceCollection services)
    {
        services.AddScoped<AccessPolicyRuleService>();
        services.AddScoped<BusinessMetricDefinitionService>();
        services.AddScoped<CompetencyQuestionService>();
        services.AddScoped<ConceptRelationService>();
        services.AddScoped<ConceptRepresentationService>();
        services.AddScoped<ConceptService>();
        services.AddScoped<DataAssetRefService>();
        services.AddScoped<DataClassificationRuleService>();
        services.AddScoped<DataOwnerRecordService>();
        services.AddScoped<JoinRelationshipService>();
        services.AddScoped<MetricImplementationService>();
        services.AddScoped<QualityConstraintService>();
        services.AddScoped<SemanticQuerySampleService>();
        services.AddScoped<TermService>();

        //var assembly = typeof(ServiceCollectionExtensions).Assembly;
        //var servicesNamespace = typeof(ConceptService).Namespace;

        //var serviceTypes = assembly.GetTypes()
        //    .Where(t => t is { IsClass: true, IsAbstract: false }
        //                && t.Namespace == servicesNamespace
        //                && GetCrudEntityType(t) is not null);

        //foreach (var implementationType in serviceTypes)
        //{
        //    var entityType = GetCrudEntityType(implementationType)!;
        //    var interfaceType = typeof(ICrudService<>).MakeGenericType(entityType);

        //    services.AddScoped(implementationType);
        //    services.AddScoped(interfaceType, sp => sp.GetRequiredService(implementationType));
        //}

        return services;
    }

    //private static Type? GetCrudEntityType(Type type)
    //{
    //    var current = type.BaseType;
    //    while (current is not null)
    //    {
    //        if (current.IsGenericType && current.GetGenericTypeDefinition() == typeof(CrudService<>))
    //            return current.GetGenericArguments()[0];
    //        current = current.BaseType;
    //    }
    //    return null;
    //}
}
