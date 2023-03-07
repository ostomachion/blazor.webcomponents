﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Ostomachion.BlazorWebComponents.Generators;

internal record class SlotInformation
{
    public string PropertyName { get; set; } = default!;
    public string PropertyTypeFullName { get; set; } = default!;
    public string? SlotName { get; set; } = default!;
    public string? RootElement { get; set; } = default!;
    public bool IsTemplated { get; set; }

    private SlotInformation() { }

    public static SlotInformation Parse(InitialPropertyInformation info, GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        var slotNameArg = info.AttributeInformation.SlotNameArgument;
        string? slotName;
        if (slotNameArg is null || slotNameArg.NameEquals is not null)
        {
            slotName = null;
        }
        else if (slotNameArg.NameColon is NameColonSyntax nameColonArg)
        {
            slotName = nameColonArg.Name.Identifier.ToString() == "slotName"
                ? (string?)context.SemanticModel.GetConstantValue(slotNameArg.Expression, cancellationToken).Value
                : null;
        }
        else
        {
            slotName = context.SemanticModel.GetConstantValue(slotNameArg.Expression, cancellationToken).Value as string;
        }

        var rootElementArg = info.AttributeInformation.RootElementArgument;
        string? rootElement = rootElementArg is null
            ? null
            : context.SemanticModel.GetConstantValue(rootElementArg.Expression, cancellationToken).Value as string;

        var isTemplatedArg = info.AttributeInformation.IsTemplatedArgument;
        bool isTemplated = isTemplatedArg is not null &&
            context.SemanticModel.GetConstantValue(isTemplatedArg.Expression, cancellationToken).Value is bool b && b;

        var propertyType = context.SemanticModel.GetTypeInfo(info.PropertySyntax.Type).Type!.ToString();

        return new SlotInformation
        {
            PropertyName = info.PropertySyntax.Identifier.ToString(),
            PropertyTypeFullName = propertyType,
            SlotName = slotName,
            RootElement = rootElement,
            IsTemplated = isTemplated,
        };
    }
}
