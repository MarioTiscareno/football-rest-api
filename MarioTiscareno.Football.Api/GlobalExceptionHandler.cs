using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mime;
using System.Text.Json;

namespace MarioTiscareno.Football.Api;

public static class GlobalExceptionHandler
{
    public static async Task HandleExceptionAsync(HttpContext context)
    {
        var exception = context.Features.Get<IExceptionHandlerPathFeature>()?.Error;

        if (exception == null)
        {
            return;
        }

        var logger = context.RequestServices.GetRequiredService<ILogger<object>>();

        var exceptionId = Guid.NewGuid();
        logger.LogError(exception, "Exception with ID {ExceptionId}", exceptionId);

        var problem = new ProblemDetails()
        {
            Type = "An internal server error has occurred, check logs for more details.",
            Status = (int)HttpStatusCode.InternalServerError,
            Extensions = new Dictionary<string, object?> { { "exceptionId", exceptionId } }
        };

        context.Response.ContentType = MediaTypeNames.Application.Json;
        context.Response.StatusCode = problem!.Status.GetValueOrDefault(
            (int)HttpStatusCode.InternalServerError
        );

        await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
    }
}
