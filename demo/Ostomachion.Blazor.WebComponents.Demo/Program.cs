using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Ostomachion.Blazor.WebComponents;
using Ostomachion.Blazor.WebComponents.Demo;
using System.Reflection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.RootComponents.Add<CustomElementRegistrarComponent>("head::after");

builder.Services.AddBlazorWebComponents(r => r.RegisterAll(Assembly.GetExecutingAssembly()));

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Build().RunAsync();
