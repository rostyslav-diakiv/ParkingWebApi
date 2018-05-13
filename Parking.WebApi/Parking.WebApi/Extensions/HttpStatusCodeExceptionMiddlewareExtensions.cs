using Microsoft.AspNetCore.Builder;

namespace Parking.WebApi.Extensions
{
    using Parking.WebApi.Middlewares;

    public static class HttpStatusCodeExceptionMiddlewareExtension
    {
        public static IApplicationBuilder UseHttpStatusCodeExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<HttpStatusCodeExceptionMiddleware>();
        }
    }
}