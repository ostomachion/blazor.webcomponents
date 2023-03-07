﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Ostomachion.BlazorWebComponents.Generators;

[Generator(LanguageNames.CSharp)]
public partial class WebComponentGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Get all classes that inherit WebComponentBase.
        // Partial classes (e.g. from Razor) will be included once from each source file.
        var webComponentsSources = context.SyntaxProvider
            .CreateSyntaxProvider(WebComponentPredicate, WebComponentInitialTransform)
            .Where(x => x is not null);

        // Gather a list of unique classes that inherit WebComponentBase.
        var distinctNames = webComponentsSources
            .Select((x, _) => new NameInfo(x!.Name, x.Namespace))
            .Collect()
            .SelectMany((x, _) => x.Distinct());

        // For each unique WebComponentBase, output a partial class with common members.
        context.RegisterSourceOutput(distinctNames, WebComponentSourceOutput.CreateCommonFile);

        // For each distinct WebComponentBase class, output a partial class with common members.
        var slotSources = webComponentsSources
            .Where(s => s!.Slots.Any())
            .Select((x, _) => new SlotSourceInformation(x!))
            .Collect()
            .SelectMany((x, _) => SlotSourceInformation.Group(x));

        // For each WebComponentBase declaration that defines slots, output a partial class with the slot properties.
        context.RegisterSourceOutput(slotSources, WebComponentSourceOutput.CreateSlotSource);

        // TODO: Handle CSS files???
    }

    private bool WebComponentPredicate(SyntaxNode n, CancellationToken _) => n is ClassDeclarationSyntax;

    private WebComponentClassInformation? WebComponentInitialTransform(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        // TODO: I tried to make this method not do more work than necessary,
        // should some of this work be split up or moved later in the process?

        var syntax = (ClassDeclarationSyntax)context.Node;
        var symbol = context.SemanticModel.GetDeclaredSymbol(syntax, cancellationToken)!;

        if (symbol.BaseType?.ToString() != "Ostomachion.BlazorWebComponents.WebComponentBase")
        {
            return null;
        }

        var slots = syntax.Members
            .OfType<PropertyDeclarationSyntax>()
            .Select(p => InitialPropertyInformation.Parse(p, context, cancellationToken))
            .OfType<InitialPropertyInformation>()
            .Select(x => SlotInformation.Parse(x, context, cancellationToken));

        return new WebComponentClassInformation
        {
            FilePath = context.Node.SyntaxTree.FilePath,
            Name = symbol.Name,
            Namespace = symbol.ContainingNamespace.ToString(),
            Slots = slots.ToArray(),
        };
    }
}
