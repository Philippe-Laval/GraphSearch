using ITSM.Web.Client.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace ITSM.Web.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.Services.AddAppServices();
            builder.Services.AddChatClient(builder.HostEnvironment.BaseAddress + "api/chat", "proxykey", "proxychat");
            builder.Services.AddAuthorizationCore();
            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();

            // Default HttpClient for static assets and other calls
            builder.Services.AddScoped(sp => new HttpClient 
            { 
                BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) 
            });

            // Typed HTTP client for ITSM ApiService (project name 'apiservice' in AppHost)
            builder.Services.AddHttpClient<ITSM.Web.Client.Services.IStatusesClient, ITSM.Web.Client.Services.StatusesClient>(client =>
            {
                client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress + "apiservice/");
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            });

            // NSwag-generated client registration (IClient)
            builder.Services.AddHttpClient("ITSMApi", client =>
            {
                client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress + "apiservice/");
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            })
            .AddTypedClient<ITSM.ApiService.Client.IClient>((http, sp) => new ITSM.ApiService.Client.Client(http.BaseAddress?.ToString() ?? "", http));

            await builder.Build().RunAsync();
        }
    }
}
