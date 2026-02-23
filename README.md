# Bigode 🧔

Bigode (Portuguese for Mustache) is a highly-performant (no regex), Ahead-Of-Time (AOT) compatible template parser for C#. It implements a focused subset of the original Mustache specification, tailored specifically for AOT compilation scenarios. By requiring explicitly typed models (`RenderModel`), Bigode ensures maximum performance and seamless native compilation without relying on reflection.

## Status

Bigode is mostly feature complete, as it currently fits all my personal needs. But its development will continue focusing on better performance and ease of use. We do accept PRs.

## Features

- AOT Compatible: Built from the ground up to support native AOT compilation in C#.
- High Performance: Designed to be incredibly fast with built-in AST tree caching.
- Relative Partials: Resolves partial files relative to the current document's location, keeping your folder structure clean.
- Custom file extension: Easily change the expected template file extensions.

## Installation

Bigode is published on NuGet.

For the core parser, you can install it via the .NET CLI:

```bash
dotnet add package Bigode
```

For ASP.NET Core Minimal API integration, install the accompanying ASP.NET package:

```bash
dotnet add package Bigode.AspNet
```

## Quick Start

To use Bigode, initialize an instance of the parser (optionally defining the target file extension) and provide a RenderModel with your data.

```cs

using Bigode;
using Bigode.Models;

var bigode = new Bigode("html"); // "html" is the default extension

```

## ASP.NET Core Integration (Minimal APIs)

Bigode is perfectly suited for high-performance, AOT-compiled ASP.NET Core Minimal APIs. The `Bigode.AspNet` package provides built-in dependency injection extensions to seamlessly use the parser inside your endpoints.

Ensure you have the `Bigode.AspNet` package installed.

Register the service using `AddBigode()`.

Inject `BigodeService` directly into your Minimal API handlers.

```cs

using Bigode.AspNet;
using Bigode.AspNet.Services;
using Bigode.Models;

var builder = WebApplication.CreateBuilder(args);

// Register the BigodeService with DI
// You can optionally configure BigodeServiceOptions here
builder.Services.AddBigode(options => {
    options.ViewsPath = "./Views"; // default value os "./Views"
    options.ViewFileExtension = "html"; // default value os "html"

#if DEBUG
    options.DisableFileCache = true; // default value is "false"
#endif
});

var app = builder.Build();

app.MapGet("/", async (BigodeService bigode) => 
{
    // Assuming your view is at "Views/Home.html"
    var page = await bigodeService.RenderViewAsync("Home", []);

    // Then render your page inside the "Views/Template.html" template/layout:
    return await bigodeService.RenderViewResultAsync("Template", new RenderModel
    {
        { "title", new ("Bigode Example Page") },
        { "content", new (page) }
    });
});

app.Run();
```

Also dont forget to include your `Views` folder in the `.csproj` so that they get copied over to the publish folder when project is published:

```xml
<ItemGroup>
    <Content Include="Views\**">
        <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </Content>
</ItemGroup>
```


## Supported Tags & Examples

Below is a comprehensive list of supported tags and scenarios, documented via examples that correspond directly to the unit tests included in the repository.

### 1. Variables

Replaces `{{name}}` with the provided string value.

```cs
var templatePath = "basic.html"; // Content: "Hello {{name}}!"
var model = new RenderModel { { "name", new("World") } };

var result = await bigode.ParseAsync(templatePath, model);
// Output: "Hello World!"
```

### 2. Comments

Comments start with a `!` and are completely ignored during rendering.

```cs
var templatePath = "ignore.html"; // Content: "Visible {{! This is hidden }} Content"
var model = new RenderModel(); // Empty model

var result = await bigode.ParseAsync(templatePath, model);
// Output: "Visible  Content"
```

### 3. Lambdas / Delegates

You can pass functions (lambdas) to manipulate the inner content of a section.

```cs
var templatePath = "lambda.html"; // Content: "{{#wrapped}}Inner Content{{/wrapped}}"
var model = new RenderModel { 
    { "wrapped", new(async (text) => $"<b>{text}</b>") } 
};

var result = await bigode.ParseAsync(templatePath, model);
// Output: "<b>Inner Content</b>"
```


### 4. Conditional Sections (Booleans)

Sections starting with `#` will only render if the provided boolean is true.

```cs
var templatePath = "condition.html"; // Content: "{{#isAdmin}}Welcome Admin{{/isAdmin}}"
var model = new RenderModel { { "isAdmin", new(true) } };

var result = await bigode.ParseAsync(templatePath, model);
// Output: "Welcome Admin"
```


### 5. Loop Sections (Arrays)

Sections evaluate to a loop when the provided model data is an array of `RenderModel` items.

```cs
var templatePath = "loop.html"; // Content: "{{#users}}Hello {{name}}{{/users}}"
var model = new RenderModel { 
    { "users", new([
        new RenderModel { { "name", new("Alice") } },
        new RenderModel { { "name", new("Bob") } }
    ])}
};

var result = await bigode.ParseAsync(templatePath, model);
// Output: "Hello AliceHello Bob"
```


### 6. Inverted Sections

Sections starting with `^` will render only if the value is missing, false, or an empty array.

```cs
var templatePath = "inverted.html"; // Content: "{{^isLogged}}Please log in{{/isLogged}}"
var model = new RenderModel { { "isLogged", new(false) } };

var result = await bigode.ParseAsync(templatePath, model);
// Output: "Please log in"
```

### 7. Missing Variables

If a variable or section is referenced but missing from the `RenderModel`, Bigode gracefully fails silently by outputting nothing.

```cs
var templatePath = "missing.html"; // Content: "Value: {{missingVar}}"
var model = new RenderModel();

var result = await bigode.ParseAsync(templatePath, model);
// Output: "Value: "
```


### 8. Partials

Partials allow you to include other templates. They are denoted by `{{> partial_name }}`. Paths are resolved relative to the calling document.

```cs
// user.html content: "<div>User: {{name}}</div>"
var templatePath = "main.html"; // Content: "Profile: {{> user }}"
var model = new RenderModel { { "name", new("Alice") } };

var result = await bigode.ParseAsync(templatePath, model);
// Output: "Profile: <div>User: Alice</div>"
```


### 9. Loop Partials

You can combine loops and partials. The partial will be rendered for each item in the array context.

```cs
// item.html content: "<li>{{ name }}</li>"
var templatePath = "list.html"; // Content: "<ul>{{#items}}{{> item }}{{/items}}</ul>"
var model = new RenderModel {
    { "items", new([
        new RenderModel { { "name", new("Apples") } },
        new RenderModel { { "name", new("Bananas") } }
    ])}
};

var result = await bigode.ParseAsync(templatePath, model);
// Output: "<ul><li>Apples</li><li>Bananas</li></ul>"
```


### 10. Nested Partials

Partials can call other partials recursively.

```cs
// leaf.html content: "I am the {{name}}."
// branch.html content: "Branch including: {{> leaf }}"
var templatePath = "root.html"; // Content: "Root starts: {{> branch }}"
var model = new RenderModel { { "name", new("leaf") } };

var result = await bigode.ParseAsync(templatePath, model);
// Output: "Root starts: Branch including: I am the leaf."
```


## Contributing

Contributions are welcome! Please feel free to submit a pull request or open an issue if you encounter any problems or have feature requests.