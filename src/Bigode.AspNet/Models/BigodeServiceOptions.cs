namespace Bigode.AspNet.Models;

/// <summary>
/// Define options on how BigodeService will operate
/// </summary>
public class BigodeServiceOptions
{
    /// <summary>
    /// Default path where views and partials are located 
    /// </summary>
    public string ViewsPath { get; set; } = "./Views";

    /// <summary>
    /// Disables the file caching, set it to true if you expect the view files to change
    /// Should be set to true during hot-reload/dev builds
    /// </summary>
    public bool DisableFileCache { get; set; } = false;

    /// <summary>
    /// View files extension without '.'. Defaults to "html".
    /// If you are using the ".mustache" or ".bigode" extensions you set set the proper extension here
    /// </summary>
    public string ViewFileExtension { get; set; } = "html";
}