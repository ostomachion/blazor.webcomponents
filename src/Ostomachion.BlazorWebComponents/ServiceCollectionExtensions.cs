using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Ostomachion.BlazorWebComponents;
public static class ServiceCollectionExtensions
{
    public static void AddBlazorWebComponents(this IServiceCollection services, RootComponentMappingCollection rootComponents, Action<CustomElementRegistrar> configure)
    {
        rootComponents.Add<CustomElementRegistrarComponent>("head::after");
        var registrar = new CustomElementRegistrar();
        services.AddSingleton(registrar);
        configure(registrar);
    }
}
