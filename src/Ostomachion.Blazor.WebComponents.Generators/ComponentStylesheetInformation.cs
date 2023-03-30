using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;

namespace Ostomachion.Blazor.WebComponents.Generators;

internal record class ComponentStylesheetInformation
{
    public string ClassName { get; set; } = default!;
    public string Namespace { get; set; } = default!;
    public SourceText? Stylesheet { get; set; }

    private ComponentStylesheetInformation() { }

    public static ComponentStylesheetInformation Parse(
        CustomElementClassInformation component,
        ImmutableArray<(string Path, SourceText Text)> stylesheetPaths,
        bool hasRazorFiles)
    {
        var stylesheet = stylesheetPaths.FirstOrDefault(x =>
            // Checks for a source file as the parent of the CSS file.
            Path.ChangeExtension(x.Path, null) == component.OriginalFilePath ||
            // If Razor source generators are disabled, also check for a source file as the sibling of the CSS file.
            !hasRazorFiles && x.Path == Path.ChangeExtension(component.OriginalFilePath, ".css"));

        return new ComponentStylesheetInformation
        {
            ClassName = component.Name,
            Namespace = component.Namespace,
            Stylesheet = stylesheet.Text,
        };
    }

    public static IEnumerable<ComponentStylesheetInformation> Group(IEnumerable<ComponentStylesheetInformation> list)
        => list.GroupBy(x => (x!.ClassName, x!.Namespace))
            .Select(g => new ComponentStylesheetInformation
            {
                ClassName = g.Key.ClassName,
                Namespace = g.Key.Namespace,
                Stylesheet = g.FirstOrDefault(x => x.Stylesheet is not null)?.Stylesheet,
            });
}
