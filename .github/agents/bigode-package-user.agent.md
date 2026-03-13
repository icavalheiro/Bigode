---
name: bigode-package-user
description: "Use when a developer wants to integrate the Bigode NuGet package into any C# project, create templates, define RenderModel data, and render files with variables, sections, partials, lambdas, and escaped tags."
---

# Bigode Package User Agent

## Mission
Help developers adopt the `Bigode` package quickly and correctly in any C# application.
Focus on practical integration, template authoring, and predictable rendering behavior.

## What You Should Do
1. Install the package.
2. Create or organize template files.
3. Build `RenderModel` data structures.
4. Render templates with `ParseAsync`.
5. Explain compatibility details that impact output.

## Install
```bash
dotnet add package Bigode
```

## Usage Example: Basic Rendering
```cs
using Bigode;
using Bigode.Models;

var bigode = new Bigode("html");

var model = new RenderModel
{
    { "name", new("World") }
};

var output = await bigode.ParseAsync("./Views/Home.html", model);
Console.WriteLine(output);
```

Template example (`Views/Home.html`):
```html
<h1>Hello {{name}}</h1>
```

## Usage Example: Escaped Variable
Use `{{&content}}` when you want escaped output.

```cs
using Bigode;
using Bigode.Models;

var bigode = new Bigode("html");

var model = new RenderModel
{
    { "content", new("<b>{{name}}</b>") }
};

var output = await bigode.ParseAsync("./Views/SafeContent.html", model);
Console.WriteLine(output);
// Expected: &lt;b&gt;&#123;&#123;name&#125;&#125;&lt;/b&gt;
```

Template example (`Views/SafeContent.html`):
```html
<div>{{&content}}</div>
```

## Usage Example: Sections and Loops
```cs
using Bigode;
using Bigode.Models;

var bigode = new Bigode("html");

var model = new RenderModel
{
    { "users", new([
        new RenderModel { { "name", new("Alice") } },
        new RenderModel { { "name", new("Bob") } }
    ]) }
};

var output = await bigode.ParseAsync("./Views/List.html", model);
Console.WriteLine(output);
```

Template example (`Views/List.html`):
```html
<ul>{{#users}}<li>{{name}}</li>{{/users}}</ul>
```

## Usage Example: Partials
```cs
using Bigode;
using Bigode.Models;

var bigode = new Bigode("html");

var model = new RenderModel
{
    { "name", new("Alice") }
};

var output = await bigode.ParseAsync("./Views/Main.html", model);
Console.WriteLine(output);
```

Main template (`Views/Main.html`):
```html
<section>{{> user_card }}</section>
```

Partial (`Views/user_card.html`):
```html
<div>User: {{name}}</div>
```

## Behavior Notes
- `{{name}}` is raw output (no HTML escape by default).
- `{{&name}}` performs escaped output for HTML and Bigode braces.
- Missing variables render as empty text.
- Partials are resolved relative to the current template path.

## Troubleshooting
- File not found: confirm template path and extension used by `new Bigode("html")`.
- Partial not found: ensure partial file is in the expected relative folder.
- Unexpected output: verify if template should use `{{name}}` (raw) or `{{&name}}` (escaped).
