using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Ostomachion.BlazorWebComponents.Generators;

internal static class WebComponentSourceOutput
{
    public static void CreateCommonFile(SourceProductionContext context, NameInfo nameInfo)
    {
        var name = nameInfo.Name;
        var namespaceName = nameInfo.Namespace;

        // TODO: Remove StylesheetUrl from here handle it in the CSS generator.
        context.AddSource($"{namespaceName}.{name}.g.cs", $$"""
                #nullable enable
                using System;
                using Microsoft.AspNetCore.Components;
                using Microsoft.AspNetCore.Components.Rendering;
                using Ostomachion.BlazorWebComponents;

                namespace {{namespaceName}};

                public partial class {{name}} : IWebComponent
                {
                    private static string? _identifier;
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
                    
                    public static string? StylesheetUrl => null;
                }
                """);
    }

    public static void CreateSlotSource(SourceProductionContext context, SlotSourceInformation item)
    {
        // TODO: Add slot template properties.
        var builder = new StringBuilder();
        builder.AppendLine($$"""
                        #nullable enable
                        using System;
                        using Microsoft.AspNetCore.Components;
                        using Microsoft.AspNetCore.Components.Rendering;
                        using Ostomachion.BlazorWebComponents;

                        namespace {{item.Namespace}};
                
                        public partial class {{item.Name}}
                        {
                            protected override void BuildRenderTreeSlots(RenderTreeBuilder builder)
                            {
                        """);

        var sequence = 0;
        foreach (var slot in item.Slots)
        {
            builder.AppendLine($$"""
                                if ((object?){{slot.PropertyName}} is not null && RenderedSlots.Contains({{ToStringLiteral(slot.PropertyName)}}))
                                {
                                    builder.OpenElement({{sequence++}}, {{ToStringLiteral(slot.RootElement)}});
                                    builder.AddAttribute({{sequence++}}, "slot", {{ToStringLiteral(slot.SlotName)}});
                                    builder.AddAttribute({{sequence++}}, "xmlns:wc", GetType().Namespace);
                                    builder.AddAttribute({{sequence++}}, $"wc:{nameof(this.{{slot.PropertyName}})}");
                        """);

            // TODO: Check for templated slots
            var templated = false;
            if (templated)
            {
                builder.AppendLine($$"""
                                    builder.AddAttribute({{sequence++}}, $"wc:{nameof(this.{{slot.PropertyName}}Template)}");
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

        foreach (var slot in item.Slots)
        {
            builder.AppendLine($$"""
                            private RenderFragment {{slot.PropertyName}}Slot => (builder) =>
                            {
                                RenderedSlots.Add({{ToStringLiteral(slot.SlotName ?? slot.PropertyName)}});
                                builder.OpenElement(0, "slot");
                                builder.AddAttribute(1, "name", {{ToStringLiteral(slot.SlotName)}});
                                builder.AddContent(2, {{ToStringLiteral(slot.SlotName)}});
                                builder.CloseElement();
                            };
                        """);
        }

        builder.Append($$"""
                        }
                        """);

        context.AddSource($"{item.Namespace}.{item.Name}-slots.g.cs", builder.ToString());
    }


    private static string ToStringLiteral(string? value) => value is null ? "null" : SymbolDisplay.FormatLiteral(value, true);
}