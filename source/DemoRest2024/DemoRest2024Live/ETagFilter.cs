using System.Text;
using System.Text.Json;
using DemoRest2024Live.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace DemoRest2024Live;

public class ETagFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var result = await next(context);

        if (context.HttpContext.Request.Method != "GET") return result;

        if (context.HttpContext.Response.StatusCode != 200) return result;

        var content = JsonSerializer.Serialize(((IValueHttpResult)result).Value);

        var eTag = ETagGenerator.GetETag(context.HttpContext.Request.Path.ToString(), Encoding.UTF8.GetBytes(content));

        if (context.HttpContext.Request.Headers.IfNoneMatch == eTag)
        {
            context.HttpContext.Response.StatusCode = 304;
            return new StatusCodeResult(304);
        }

        context.HttpContext.Response.Headers.Append("ETag", new[] { eTag });
        return result;
    }
}