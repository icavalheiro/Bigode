using System.Net.Http.Headers;
using Bigode.AspNet.Models;
using Bigode.AspNet.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Bigode.AspNet;

/// <summary>
/// Adds extensions methods to asp.net setup classes
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Add bigode service for rendering bigode html files
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configure"></param>
    public static void AddBigode(this IServiceCollection services, Action<BigodeServiceOptions>? configure = null)
    {
        var options = new BigodeServiceOptions();
        if (configure is not null)
            configure(options);

        services.AddSingleton(options);
        services.AddSingleton<BigodeService>();
    }
}
