using Bigode.AspNet;
using Bigode.AspNet.Services;
using Bigode.Models;

var builder = WebApplication.CreateSlimBuilder(args);
builder.Services.AddBigode(configure =>
{
    configure.ViewsPath = "./Views";
    configure.ViewFileExtension = "html";

#if DEBUG
    configure.DisableFileCache = true;
#endif
});

var app = builder.Build();
app.UseStaticFiles();

app.MapGet("/", async (BigodeService bigodeService) =>
{
    var page = await bigodeService.RenderViewAsync("Home", []);
    return await bigodeService.RenderViewResultAsync("Template", new RenderModel
    {
        { "title", new ("Bigode Example Page") },
        { "content", new (page) }
    });
});

app.Run();
