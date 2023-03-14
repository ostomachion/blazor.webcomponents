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

    public static void CreateSlotSource(SourceProductionContext context, SlotSourceInformation item)
    {
        var builder = new StringBuilder();
        builder.AppendLine($$"""
                        #nullable enable
                        using Microsoft.AspNetCore.Components;
                        using Microsoft.AspNetCore.Components.Rendering;
                        using Ostomachion.Blazor.WebComponents;
                        using System;
                        using System.Collections.Immutable;
                        using System.Runtime.CompilerServices;

                        namespace {{item.Namespace}};
                
                        public partial class {{item.Name}}
                        {
                        """);

        var sequence = 0;

        // Output slot properties.
        foreach (var slot in item.Slots)
        {
            builder.AppendLine($$"""
                            /// <summary>
                            /// Represents the rendered <c>slot</c> element in the light DOM for the {{slot.PropertyName}} property.
                            /// </summary>
                            private RenderFragment {{slot.PropertyName}}Slot => (builder) =>
                            {
                                RenderedSlots.Add({{ToStringLiteral(slot.PropertyName)}});
                                builder.OpenElement(0, "slot");
                                builder.AddAttribute(1, "name", {{ToStringLiteral(slot.SlotName ?? slot.PropertyName)}});
                                builder.AddContent(2, {{ToStringLiteral(slot.DefaultText ?? slot.SlotName ?? slot.PropertyName)}});
                                builder.CloseElement();
                            };

                        """);
        }

        // Output BuildRenderTreeSlots method.
        builder.AppendLine($$"""
                            /// <inheritdoc/>
                            protected override void BuildRenderTreeSlots(RenderTreeBuilder builder)
                            {
                        """);

        foreach (var slot in item.Slots)
        {
            builder.AppendLine($$"""
                                if ((object?){{slot.PropertyName}} is not null && RenderedSlots.Contains({{ToStringLiteral(slot.PropertyName)}}))
                                {
                                    builder.OpenElement({{sequence++}}, {{ToStringLiteral(slot.RootElement)}});
                                    builder.AddAttribute({{sequence++}}, "slot", {{ToStringLiteral(slot.SlotName)}});
                                    builder.AddAttribute({{sequence++}}, "xmlns:ce", GetType().Namespace);
                                    builder.AddAttribute({{sequence++}}, $"ce:{nameof(this.{{slot.PropertyName}})}");
                        """);

            if (slot.IsTemplated)
            {
                builder.AppendLine($$"""
                                    builder.AddAttribute({{sequence++}}, $"ce:{nameof(this.{{slot.PropertyName}}Template)}");
                                    if (this.{{slot.PropertyName}}Template is null)
                                    {
                                        builder.AddContent({{sequence++}}, this.{{slot.PropertyName}});
                                    }
                                    else
                                    {
                                        builder.AddContent({{sequence++}}, this.{{slot.PropertyName}}Template, this.{{slot.PropertyName}});
                                    }
                        """);
            }
            else
            {
                builder.AppendLine($$"""
                                    builder.AddContent({{sequence++}}, this.{{slot.PropertyName}});
                        """);
            }

            builder.AppendLine($$"""
                                    builder.CloseElement();
                                }
                        """);
        }

        builder.AppendLine($$"""
                            }

                        """);

        // Output Slot property.
        builder.AppendLine($$"""
                            /// <inheritdoc/>
                            protected override SlotLookup Slot => new(new Dictionary<string, RenderFragment>
                                {
                        """);

        foreach (var slot in item.Slots)
        {
            builder.AppendLine($$"""
                                    [{{ToStringLiteral(slot.PropertyName)}}] = {{slot.PropertyName}}Slot,
                                    ["this.{{ToStringLiteral(slot.PropertyName, quote: false)}}"] = {{slot.PropertyName}}Slot,
                        """);
        }

        builder.AppendLine($$"""
                                }.ToImmutableDictionary());

                        """);

        // Output IsTemplateDefined method.
        builder.AppendLine($$"""
                            /// <inheritdoc/>
                            protected override bool IsTemplateDefined(object? property, [CallerArgumentExpression(nameof(property))]string propertyName = default!)
                                => propertyName switch
                                {
                        """);

        foreach (var slot in item.Slots.Where(x => x.IsTemplated))
        {
            builder.AppendLine($$"""
                                    {{ToStringLiteral(slot.PropertyName)}} or "this.{{ToStringLiteral(slot.PropertyName, quote: false)}}" => {{slot.PropertyName}}Template is not null,
                        """);
        }

        builder.AppendLine($$"""
                                    _ => throw new ArgumentException($"'{propertyName}' is not a templated property", nameof(property))
                                };
                        """);

        // End class.
        builder.AppendLine($$"""
                        }
                        """);

        context.AddSource($"{item.Namespace}.{item.Name}-slots.g.cs", builder.ToString());
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