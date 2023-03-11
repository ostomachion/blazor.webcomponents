using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Ostomachion.BlazorWebComponents;

/// <summary>
/// Extensions methods for <see cref="IServiceCollection"/> to help set up an app to use custom elements and web components.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Sets up blazor web components on the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure.</param>
    /// <param name="rootComponents">The <see cref="RootComponentMappingCollection"/> for the app.</param>
    /// <param name="configure">An action delegate to configure the provided <see cref="CustomElementRegistrar"/>.</param>
    public static void AddBlazorWebComponents(this IServiceCollection services, RootComponentMappingCollection rootComponents, Action<CustomElementRegistrar> configure)
    {
        rootComponents.Add<CustomElementRegistrarComponent>("head::after");
        var registrar = new CustomElementRegistrar();
        services.AddSingleton<ICustomElementRegistrar>(registrar);
        configure(registrar);
    }
}
