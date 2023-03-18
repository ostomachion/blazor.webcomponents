# BlazorWebComponents

A simple library that allows Blazor components to be rendered as real
standards-based Web Components using custom elements, shadow DOM, and HTML
templates.

Still in development, but mostly tested and functional for Blazor WebAssembly
projects.

## TL;DR

1. Follow the [installation](#installation) and [setup](#setup) sections.
2. Modify the call to `AddBlazorWebComponents` to the following:
    ```csharp
    builder.Services.AddBlazorWebComponents(r => r.RegisterAll(Assembly.GetExecutingAssembly())});
    ```
3. Add a new Razor Component to your project:
    
    *MyComponent.razor*
    ```razor
    @inherits CustomElementBase
    <p class="shadow">Shadow: @Value</p>
    <p class="light">Light: <Slot Name="value" For="Value">missing value</Slot></p>
    ```

    *MyComponent.razor.cs*
    ```csharp
    namespace My.Namespace;

    [CustomElement("my-component")]
    public class MyComponent : WebComponent
    {
        [Parameter]
        [EditorRequired]
        public string Value { get; set; } = default!;
    }
    ```

    *MyComponent.razor.css*
    ```css
    .shadow { background: lightgray; }
    .light { background: lightyellow; }
    ```

4. Add the component to the main page.

    ```razor
    <MyComponent Value="Hello, world!" />
    ```

That's it! You've got a full standards-based web component from Blazor!

*Rendered output*
```html
<my-component>
    #shadowroot (open)
      <style>
        .shadow { background: lightgray; }
        .light { background: lightyellow; }
      </style>
      <p class="shadow">Shadow: Hello, world!</p>
      <p class="light">Light: <slot name="value">missing value</slot></p>

    <span slot="value">Hello, world!</span>
</my-component>
```

## Installation

```
dotnet add package Ostomachion.Blazor.WebComponents
```

## Setup

First, follow the installation and setup instructions for
[Ostomachion.Blazor.ShadowDom](https://github.com/ostomachion/blazor.shadowdom/blob/main/README.md).
(This will be included automatically in a future release.)

### WebAssembly

In `wwwroot/index.html`, add the following script:

```html
<script src="_content/Ostomachion.Blazor.WebComponents/blazor-web-components.js"></script>
```

In `Program.cs`, add the following lines:

```csharp
builder.RootComponents.Add<CustomElementRegistrarComponent>("head::after");
builder.Services.AddBlazorWebComponents();
```

### Server

**Note:** Blazor Web Components has not yet been thoroughly tested with Blazor
server.

In `Pages/_Host.html`, add the following script:

```html
<script src="_content/Ostomachion.Blazor.WebComponents/blazor-web-components.js"></script>
```

In `Program.cs`, add the following lines:

```csharp
builder.Services.AddBlazorWebComponents();
```

In `Pages/_Host.cs` add the following line to the end of the `head` element:

```razor
<component type="typeof(CustomElementRegistrarComponent)" render-mode="ServerPrerendered" />
```

### MAUI

**Note:** Blazor Web Components has not yet been thoroughly tested with
MAUI/Blazor WebView.

In `wwwroot/index.html`, add the following script:

```html
<script src="_content/Ostomachion.Blazor.WebComponents/blazor-web-components.js"></script>
```

In `MauiProgram.cs` add the following line:

```csharp
builder.Services.AddBlazorWebComponents();
```

In `MainPage.xaml`, in the `BlazorWebView.RootComponents` element, add the
following line:

```xml
<RootComponent Selector="head::after" ComponentType="{x:Type Ostomachion.BlazorWebComponents.CustomElementRegistrarComponent}" />
```

## Custom Elements

### Creating a Custom Element Component

**Note:** Custom elements must be [registered](#registering-custom-elements)
before they can be rendered on a page.

**Important:** Please read and understand the [notes on technical limitations](#notes-on-technical-limitations).

Any component class that inherits `Ostomachion.WebComponents.CustomElementBase`
will be rendered inside a [custom element](https://developer.mozilla.org/en-US/docs/Web/Web_Components/Using_custom_elements).

By default, custom elements are rendered as **autonomous custom elements** with
an identifier generated from the component's class and namespace.
[(Example)](#basic-custom-element)

The default identifier can be specified using a `CustomElementAtrribute`.
[(Example)](#custom-default-identifier)

A **customized built-in element** can be created using a `CustomElementAttribute`.
[(Example)](#customized-built-in-element)

If a reference to the generated custom element is stored in the `Host` property
of the component.

Attributes on the generated custom element can be set using the `HostAttributes`
property of the component. [(Example)](#adding-host-attribute)

### Registering Custom Elements

Before a custom element component can be rendered on a page, it must be
registered by passing an action to the call to `AddBlazorWebComponents`.
[(Example)](#basic-registration)

To avoid identifier collisions and to allow more customization, an identifier
can be registered with the component which will override any default identifier
defined by the component itself.
[(Example)](#identifier-registration)

For convenience, all custom elements in an assembly can be registered at once
using their default identifiers by calling `RegisterAll`. Any custom elements that
have already been registered will be skipped by `RegisterAll`.
[(Example)](#register-all-custom-elements)

## Web Components

**Web Components** are extensions of custom elements with many extra features.
Everything in the [Custom Elements](#custom-elements) section applies to web
components including [registration](#registering-custom-elements).

In addition to being wrapped in a [custom element](https://developer.mozilla.org/en-US/docs/Web/Web_Components/Using_custom_elements),
web components are also rendered in a [shadow DOM](https://developer.mozilla.org/en-US/docs/Web/Web_Components/Using_shadow_DOM)
and make use of [templates and slots](https://developer.mozilla.org/en-US/docs/Web/Web_Components/Using_templates_and_slots).

### Creating a Web Component

Any component class that inherits `Ostomachion.WebComponents.WebComponentBase`
will be rendered inside a shadow DOM attached to custom element.
[(Example)](#basic-web-component)

By default, an open shadow root is attached to the host element. The shadow root
mode can be specified by overriding the `ShadowRootMode` property on the
component.
[(Example)](#shadow-root-mode)

Any CSS file associated with the class will be automatically encapsulated in the
shadow root.
[(Example)](#web-component-styling)

### Slots

By default, the content of a web component is rendered in the shadow DOM and
generally encapsulated from CSS and JavaScript outside the component.

TODO

## Notes on CSS

Since the host element name of a custom element component can vary, CSS selectors
become fragile. As a workaround, this library adds custom namespaced elements to
custom elements. The attribute name is equal to the class name in lowercase with
a namespace equal to the namespace of the class in lowercase. This not only
provides a unique name for each component, it more closely matches the source
Razor file.
[(Example)](#css-selectors)

## ⚠️ Notes on Technical Limitations ⚠️

**Be aware of the following limitations:**
- A component that inherits either `CustomElementBase` or `WebComponentBase`
  **MUST** declare the base class in the `.cs` file (*and* the `.razor` file if one
  exists).
- A `CustomElementAttribute` **MUST** be applied to the class in the `.cs` file
  and **not** in the `.razor` file.
- Any properties with a `SlotAttribute` **MUST** be defined in the `.cs` file and
  not in the `.razor` file.

Adding `<UseRazorSourceGenerator>false</UseRazorSourceGenerator>` to the `.csproj`
should in theory fix these limitations, but this is not a priority to support.
Please report any issue if you need this feature.

**Explanation:** Some of the functionality of this library is implemented as a C#
source generator. Unfortunately, there is currently no great way to get source
generators to work will with Razor files. This means that the Blazor Web Component
source generator will not work properly if you add certain features to the
`.razor` side of a component rather than the `.cs` side.

## Examples

### Basic Custom Element

*Test.razor*
```razor {data-filename="Test.razor"}
@inherits CustomElementBase
<p>Hello, world!</p>
```

*Example.razor.cs*
```csharp
namespace Example;

public class Test : CustomElementBase { }
```

*Rendered output*
```html
<example-customelementbase>
  <p>Hello, world!</p>
</example-customelementbase>
```

### Custom Default Identifier

The default identifier can be changed by adding a `CustomElementAttribute` to the
class.

*Test.razor*
```razor
@inherits CustomElementBase
<p>Hello, world!</p>
```

*Example.razor.cs*
```csharp
namespace Example;

[CustomElement("docs-test")]
public class Test : CustomElementBase { }
```

*Rendered output*
```html
<docs-test>
  <p>Hello, world!</p>
</docs-test>
```

### Customized Built-In Element

A **customized built-in element** can be created by adding a
`CustomElementAttribute` to the class.

*Test.razor*
```razor
@inherits CustomElementBase
<p>Hello, world!</p>
```

*Example.razor.cs*
```csharp
namespace Example;

[CustomElement("docs-test", Extends = "div")]
public class Test : CustomElementBase { }
```

*Rendered output*
```html
<div is="docs-test">
  <p>Hello, world!</p>
</div>
```

### Adding Host Attributes

Attributes can be set on the generated custom element using the `HostAttributes`
property.

*Test.razor*
```razor
@inherits CustomElementBase
@HostAttributes["id"] = "host"
<p>Hello, world!</p>
```

*Example.razor.cs*
```csharp
namespace Example;

[CustomElement("docs-test")]
public class Test : CustomElementBase { }
```

*Rendered output*
```html
<docs-test id="host">
  <p>Hello, world!</p>
</docs-test>
```

### Basic Registration

A custom element must be registered before it can be rendered on a page.

*Test.razor*
```razor
@inherits CustomElementBase
<p>Hello, world!</p>
```

*Example.razor.cs*
```csharp
namespace Example;

[CustomElement("docs-test")]
public class Test : CustomElementBase { }
```

*Program.cs / MauiProgram.cs*
```csharp
...
builder.Services.AddBlazorWebComponents(r =>
{
    r.Register<Test>();
});
...
```

*Rendered output*
```html
<docs-test>
  <p>Hello, world!</p>
</docs-test>
```

### Identifier Registration

The default identifier of a custom element can be overridden at registration.

*Test.razor*
```razor
@inherits CustomElementBase
<p>Hello, world!</p>
```

*Example.razor.cs*
```csharp
namespace Example;

[CustomElement("docs-test")]
public class Test : CustomElementBase { }
```

*Program.cs / MauiProgram.cs*
```csharp
...
builder.Services.AddBlazorWebComponents(r =>
{
    r.Register<Test>("my-element");
});
...
```

*Rendered output*
```html
<my-element>
  <p>Hello, world!</p>
</my-element>
```

### Register All Custom Elements

The `RegisterAll` method will register all custom elements in a given assembly
that have not already been registered.

*Test.razor*
```razor
@inherits CustomElementBase
<p>Hello, world!</p>
```

*Example.razor.cs*
```csharp
namespace Example;

[CustomElement("docs-test")]
public class Test : CustomElementBase { }
```

*Program.cs / MauiProgram.cs*
```csharp
...
builder.Services.AddBlazorWebComponents(r =>
{
    r.RegisterAll(Assembly.GetExecutingAssembly());
});
...
```

*Rendered output*
```html
<docs-test>
  <p>Hello, world!</p>
</docs-test>
```

### Basic WebComponent

*Test.razor*
```razor
@inherits WebComponentBase
<p>Hello, world!</p>
```

*Example.razor.cs*
```csharp
namespace Example;

[custom-element("docs-test")]
public class Test : WebComponentBase { }
```

*Rendered output*
```html
<docs-test>
  #shadow-root (open)
    <p>Hello, world!</p>

</docs-test>
```

### Shadow Root Mode

*Test.razor*
```razor
@inherits WebComponentBase
<p>Hello, world!</p>
```

*Example.razor.cs*
```csharp
namespace Example;

[custom-element("docs-test")]
public class Test : WebComponentBase
{
    public override ShadowRootMode ShadowRootMode => ShadowRootMode.Closed;
}
```

*Rendered output*
```html
<docs-test>
  #shadow-root (closed)
    <p>Hello, world!</p>

</docs-test>
```

### Web Component Styling

*Test.razor*
```razor
@inherits WebComponentBase
<p>Hello, world!</p>
```

*Example.razor.cs*
```csharp
namespace Example;

[custom-element("docs-test")]
public class Test : WebComponentBase
{
    public override ShadowRootMode ShadowRootMode => ShadowRootMode.Closed;
}
```

*Example.razor.css*
```css
p {
    color: red;
}
```

*Rendered output*
```html
<docs-test>
  #shadow-root (open)
    <style>
        p {
            color: red;
        }
    </style>
    <p>Hello, world!</p>

</docs-test>
```

### CSS Selectors

*Test.razor*
```razor
@inherits WebComponentBase
Hello, world!
```

*Example.razor.cs*
```csharp
namespace Example;

[custom-element("docs-test")]
public class Test : WebComponentBase { }
```

*Index.razor*
```razor
<Test />
```

*Index.razor.css***
```css
@namespace ce 'example';

[ce|test] {
    border: 1px solid black;
}
```

---

<small style="font-size: 70%; float: right">*Special thanks to [Poor Egg Productions](https://pooregg.productions) for the icon!*</small>