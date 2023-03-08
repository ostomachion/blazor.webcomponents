using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Ostomachion.BlazorWebComponents;
using Ostomachion.BlazorWebComponents.Demo;
using System.Reflection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.RootComponents.RegisterAllCustomElements(Assembly.GetExecutingAssembly());

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddSingleton<IJSComponentConfiguration>(_ => builder.RootComponents);

builder.Build().RunAsync();
