using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using MudBlazorApp;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure HttpClient to use the API URL
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5200/") });
builder.Services.AddMudServices();

await builder.Build().RunAsync();
