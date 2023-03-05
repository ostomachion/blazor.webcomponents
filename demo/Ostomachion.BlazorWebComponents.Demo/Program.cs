using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Ostomachion.BlazorWebComponents;
using Ostomachion.BlazorWebComponents.Demo;
using Ostomachion.BlazorWebComponents.Demo.Demos.ComposedComposedPath;
using Ostomachion.BlazorWebComponents.Demo.Demos.DefinedPseudoClass;
using Ostomachion.BlazorWebComponents.Demo.Demos.EditWord;
using Ostomachion.BlazorWebComponents.Demo.Demos.ElementDetails;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.RootComponents.RegisterWebComponent<ClosedShadow>("closed-shadow");
builder.RootComponents.RegisterWebComponent<OpenShadow>("open-shadow");
builder.RootComponents.RegisterWebComponent<SimpleCustom>("simple-custom");
builder.RootComponents.RegisterWebComponent<EditWord>("edit-word");
builder.RootComponents.RegisterWebComponent<PersonDetails>("person-details");
builder.RootComponents.RegisterWebComponent<ElementDetails>("element-details");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddSingleton<IJSComponentConfiguration>(sp => builder.RootComponents);

builder.Build().RunAsync();
