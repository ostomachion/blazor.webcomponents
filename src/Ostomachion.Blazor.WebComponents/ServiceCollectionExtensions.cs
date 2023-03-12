using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Ostomachion.Blazor.WebComponents;

/// <summary>
/// Extensions methods for <see cref="IServiceCollection"/> to help set up an app to use custom elements and web components.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Sets up blazor web components on the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure.</param>
    /// <param name="configure">An action delegate to configure the provided <see cref="ICustomElementRegistrar"/>.</param>
    public static void AddBlazorWebComponents(this IServiceCollection services, Action<ICustomElementRegistrar> configure)
    {
        var registrar = new CustomElementRegistrar();
        services.AddSingleton<ICustomElementRegistrar>(registrar);
        configure(registrar);
    }
}
