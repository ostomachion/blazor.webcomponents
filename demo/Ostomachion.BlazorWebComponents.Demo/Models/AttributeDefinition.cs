using Microsoft.AspNetCore.Components;

namespace Ostomachion.BlazorWebComponents.Demo.Models;

public record class AttributeDefinition(string Name, RenderFragment Definition);
