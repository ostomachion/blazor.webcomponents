using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Ostomachion.BlazorWebComponents;
using Ostomachion.BlazorWebComponents.Demo;
using System.Reflection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazorWebComponents(builder.RootComponents, r => r.RegisterAll(Assembly.GetExecutingAssembly()));

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Build().RunAsync();
