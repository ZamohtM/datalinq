using E.DataLinq.Code.Middleware.Authentication;
using Microsoft.AspNetCore.Builder;

namespace E.DataLinq.Code.Extensions.DependencyInjection;
static public class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseDataLinqCodeAuthentication(this IApplicationBuilder app)
    {
        app.UseMiddleware<DataLinqCodeTokenAuthenticationMiddleware>();
        return app;
    }
}
