using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Ostomachion.Blazor.WebComponents.Generators;

internal static class CustomElementSourceOutput
{
    public static void CreateCommonFile(SourceProductionContext context, NameInfo nameInfo)
    {
        var name = nameInfo.Name;
        var namespaceName = nameInfo.Namespace;

        context.AddSource($"{namespaceName}.{name}.g.cs", $$"""
                #nullable enable
                using System;
                using Microsoft.AspNetCore.Components;
                using Microsoft.AspNetCore.Components.Rendering;
                using Ostomachion.Blazor.WebComponents;

                namespace {{namespaceName}};

                public partial class {{name}} : ICustomElement
                {
                    private static string? _identifier;

                    /// <inheritdoc/>
                    public static string? Identifier
                    {
                        get => _identifier;
                        set
                        {
                            if (_identifier is not null)
                            {
                                throw new InvalidOperationException("Identifier has already been set.");
                            }
                            _identifier = value;
                        }
                    }
                
                    /// <inheritdoc/>
                    public static string? LocalName => {{ToStringLiteral(nameInfo.LocalName)}};
                }
                """);
    }

    public static void CreateStylesheetSource(SourceProductionContext context, ComponentCssInformation item)
    {
        context.AddSource($"{item.Namespace}.{item.ClassName}-stylesheet.g.cs", $$"""
                        #nullable enable
                        using Ostomachion.Blazor.WebComponents;
                        using System;
                        using System.IO;

                        namespace {{item.Namespace}};
                
                        public partial class {{item.ClassName}} : IWebComponent
                        {
                            /// <inheritdoc/>
                            public static string? Stylesheet => {{ToStringLiteral(item.Stylesheet?.ToString())}};
                        }
                        """);
    }

    private static string ToStringLiteral(string? value, bool quote = true) => value is null ? "null" : SymbolDisplay.FormatLiteral(value, quote);
}