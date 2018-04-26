using System.Threading.Tasks;
using NiceNuget.Api.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace NiceNuget.Api.Infrastructure {
    public class RefreshCacheMiddleware {

        private readonly IPackageCache _cache;
        private readonly ILogger _logger;

        private readonly RequestDelegate _next;

        public RefreshCacheMiddleware(RequestDelegate next, IPackageCache cache, ILogger<RefreshCacheMiddleware> logger) {
            _cache = cache;
            _logger = logger;

            _next = next;
        }

        public async Task Invoke(HttpContext context) {
            await _cache.RefreshCache();
            await _next(context);
        }

    }

    public static class RefreshCacheMiddlewareExtensions {
        public static IApplicationBuilder UseRefreshCacheMiddleware(this IApplicationBuilder app) {
            app.UseMiddleware<RefreshCacheMiddleware>();
            return app;
        }
    }
}