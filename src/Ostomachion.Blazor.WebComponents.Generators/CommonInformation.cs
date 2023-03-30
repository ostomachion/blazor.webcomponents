using System.ComponentModel;

namespace Ostomachion.Blazor.WebComponents.Generators;

internal record class CommonInformation
{
    public string Name { get; set; } = default!;
    public string Namespace { get; set; } = default!;
    public string? LocalName { get; set; }
    public string? ModulePath { get; set; }

    private CommonInformation() { }

    public static CommonInformation Parse(CustomElementClassInformation customElement, bool hasRazorFiles, string projectDirectory)
    {
        string? modulePath = null;

        var childPath = customElement.OriginalFilePath + ".js";
        if (File.Exists(childPath))
        {
            // Note: These files will only be automatically accessible by the browser if the
            // OriginalFilePath is a Razor component.
            modulePath = childPath;
        }
        else if (!hasRazorFiles)
        {
            // Note: Files found by this path will not be automatically accessible by the browser
            // like the collocated files from the "childPath" branch.
            var siblingPath = Path.ChangeExtension(customElement.OriginalFilePath, ".js");
            if (File.Exists(siblingPath))
            {
                modulePath = siblingPath;
            }
        }

        if (modulePath is not null)
        {
            modulePath = new Uri(projectDirectory).MakeRelativeUri(new Uri(modulePath)).ToString();
        }

        return new CommonInformation
        {
            Name = customElement.Name,
            Namespace = customElement.Namespace,
            LocalName = customElement.LocalName,
            ModulePath = modulePath,
        };
    }

    public static IEnumerable<CommonInformation> Group(IEnumerable<CommonInformation> list)
        => list.GroupBy(x => (x!.Name, x!.Namespace))
            .Select(g => new CommonInformation
            {
                Name = g.Key.Name,
                Namespace = g.Key.Namespace,
                LocalName = g.FirstOrDefault(x => x!.LocalName is not null)?.LocalName,
                ModulePath = g.FirstOrDefault(x => x!.ModulePath is not null)?.ModulePath,
            });
}
