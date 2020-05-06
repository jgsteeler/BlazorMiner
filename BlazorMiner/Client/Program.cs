using BlazorMiner.Client.Services;
using BlazorMiner.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BlazorMiner.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddHttpClient("BlazorMiner.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            // Supply HttpClient instances that include access tokens when making requests to the server project
            builder.Services.AddTransient(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("BlazorMiner.ServerAPI"));

            builder.Services.AddApiAuthorization();

            builder.Services.AddSingleton<MinerClient>();
            builder.Services.AddSingleton<RoomPinValidator>();
            builder.Services.AddSingleton(MinerClient.HubConnectionFactory);
            var host = builder.Build();
            var hubConnection = await host.Services.GetRequiredService<Task<HubConnection>>();
            if (hubConnection != null)
                builder.Services.AddSingleton(hubConnection);
            host = builder.Build();
            await host.RunAsync();
        }
    }
}
