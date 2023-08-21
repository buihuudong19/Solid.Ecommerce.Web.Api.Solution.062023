using Microsoft.Extensions.Primitives;

namespace Solid.Ecommerce.WebApi.Security;

public class SecurityHeaders
{
    private readonly RequestDelegate next;
    public SecurityHeaders(RequestDelegate next)
    {
        this.next = next;
    }
    public Task Invoke(HttpContext context)
    {
        // add any HTTP response headers you want here
        /*
        context.Response.Headers.Add(
          "super-secure", new StringValues("enable"));
        */
        //Them tieu de Strict-Transport-Security
        context.Response.Headers.Add("Strict-Transport-Security","max-age=31536000");
        context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'");
        context.Response.Headers.Add("X-Content-Type-Option", "nosniff");
        context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
        context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
        context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");

        return next(context);
    }


}
