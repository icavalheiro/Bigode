using Bigode.AspNet.Models;
using Bigode.Models;
using Microsoft.AspNetCore.Http;

namespace Bigode.AspNet.Services;

/// <summary>
/// Service that exposes methods to render views using Bigode
/// </summary>
public class BigodeService(BigodeServiceOptions options)
{
    private readonly Bigode bigode = new(options.ViewFileExtension, options.DisableFileCache);

    /// <summary>
    /// Renders the current view and returns a string with its result
    /// </summary>
    /// <param name="viewName">Name of the view file</param>
    /// <param name="model">Model to be used during rendering</param>
    /// <returns>The rendered view as string</returns>
    public async Task<string> RenderViewAsync(string viewName, RenderModel model)
    {
        var path = Path.GetFullPath(Path.Join(options.ViewsPath, $"{viewName}.{options.ViewFileExtension}"));
        return await bigode.ParseAsync(path, model);
    }

    /// <summary>
    /// Renders the current view and returns a HTTP IResult
    /// </summary>
    /// <param name="viewName">Name of the view file</param>
    /// <param name="model">Model to be used during rendering</param>
    /// <returns>The rendered view as IResult "text/html"</returns>
    public async Task<IResult> RenderViewResultAsync(string viewName, RenderModel model)
    {
        string view = await RenderViewAsync(viewName, model);
        return TypedResults.Content(view, "text/html");
    }
}