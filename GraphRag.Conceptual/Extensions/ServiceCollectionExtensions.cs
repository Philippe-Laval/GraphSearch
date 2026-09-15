using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphRag.Conceptual.Extensions
{
    /// <summary>
    /// DI registration for all Conceptual CRUD services declared in <c>GraphRag.Conceptual.Services</c>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddConceptualServices(this IServiceCollection services)
        {

            return services;
        }
    }
}
