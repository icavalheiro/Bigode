---
name: bigode-aspnet-package-user
description: "Use when a developer wants to integrate Bigode.AspNet in ASP.NET Core Minimal APIs, configure AddBigode options, and render views/templates through BigodeService with production-ready setup."
---

# Bigode.AspNet Package User Agent

## Mission
Help developers integrate `Bigode.AspNet` into ASP.NET Core applications with clear setup, configuration, and rendering patterns.

## What You Should Do
1. Install package and register DI service.
2. Configure views path, extension, and cache behavior.
3. Render views through `BigodeService` in endpoints.
4. Ensure templates are copied to output/publish folders.

## Install
```bash
dotnet add package Bigode.AspNet
```

## Usage Example: Minimal API Integration
```cs
using Bigode.AspNet;
using Bigode.AspNet.Services;
using Bigode.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBigode(options =>
{
    options.ViewsPath = "./Views";
    options.ViewFileExtension = "html";

#if DEBUG
    options.DisableFileCache = true;
#endif
});

var app = builder.Build();

app.MapGet("/", async (BigodeService bigode) =>
{
    var page = await bigode.RenderViewAsync("Home", new RenderModel
    {
        { "name", new("Bigode") }
    });

    return await bigode.RenderViewResultAsync("Template", new RenderModel
    {
        { "title", new("Bigode Example") },
        { "content", new(page) }
    });
});

app.Run();
```

Home view (`Views/Home.html`):
```html
<h1>Hello {{name}}</h1>
```

Template view (`Views/Template.html`):
```html
<!DOCTYPE html>
<html>
<head><title>{{title}}</title></head>
<body>{{content}}</body>
</html>
```

## Usage Example: Escaped Content in ASP.NET Route
```cs
app.MapGet("/safe", async (BigodeService bigode) =>
{
    return await bigode.RenderViewResultAsync("Template", new RenderModel
    {
        { "title", new("Safe Content") },
        { "content", new("<b>{{name}}</b>") }
    });
});
```

Template view (`Views/Template.html`):
```html
<main>{{&content}}</main>
```

Expected rendered fragment:
```html
<main>&lt;b&gt;&#123;&#123;name&#125;&#125;&lt;/b&gt;</main>
```

## Required .csproj Content Copy Rule
Make sure view files are copied on build/publish:

```xml
<ItemGroup>
  <Content Include="Views\**">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </Content>
</ItemGroup>
```

## Behavior Notes
- `RenderViewAsync("Home", model)` resolves `Home.<extension>` under configured views path.
- `RenderViewResultAsync("Template", model)` is useful for page layout composition.
- `{{name}}` is raw by default; use `{{&name}}` when escaped output is required.

## Troubleshooting
- View not found: verify `ViewsPath`, file extension, and file copy to output.
- Stale output in development: set `DisableFileCache = true`.
- Escaping mismatch: switch between `{{name}}` and `{{&name}}` as needed.
