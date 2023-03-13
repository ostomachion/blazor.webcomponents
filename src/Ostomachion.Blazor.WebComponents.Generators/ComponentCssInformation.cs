using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;

namespace Ostomachion.Blazor.WebComponents.Generators;

internal record class ComponentCssInformation
{
    public string ClassName { get; set; } = default!;
    public string Namespace { get; set; } = default!;
    public SourceText? Stylesheet { get; set; }

    private ComponentCssInformation() { }

    public static ComponentCssInformation Parse(CustomElementClassInformation component, ImmutableArray<(string Path, SourceText Text)> styledComponentPaths, bool hasRazorFiles)
    {
        var match = styledComponentPaths.FirstOrDefault(x =>
            // Checks for a source file as the parent of the CSS file.
            Path.ChangeExtension(x.Path, null) == component.OriginalFilePath ||
            // If Razor source generators are enabled, also check for a source file as the sibling of the CSS file.
            !hasRazorFiles && x.Path == Path.ChangeExtension(component.OriginalFilePath, ".css"));

        return new ComponentCssInformation
        {
            ClassName = component.Name,
            Namespace = component.Namespace,
            Stylesheet = match.Text,
        };
    }

    public static IEnumerable<ComponentCssInformation> Group(IEnumerable<ComponentCssInformation> list)
        => list.GroupBy(x => (x!.ClassName, x!.Namespace))
            .Select(g => new ComponentCssInformation
            {
                ClassName = g.Key.ClassName,
                Namespace = g.Key.Namespace,
                Stylesheet = g.FirstOrDefault(x => x.Stylesheet is not null)?.Stylesheet
            });
}
