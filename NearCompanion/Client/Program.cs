using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using NearCompanion.Client;
using NearCompanion.Client.Services;
using NearCompanion.Client.Services.Interfaces;
using Radzen;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var baseUri = new Uri(builder.HostEnvironment.BaseAddress);
var httpClient = new HttpClient { BaseAddress = baseUri };
var blockService = new BlockService(httpClient);
builder.Services.AddScoped(sp => httpClient);
builder.Services.AddScoped<IBlockService>(bs => blockService);
builder.Services.AddScoped<DialogService>();


await builder.Build().RunAsync();
