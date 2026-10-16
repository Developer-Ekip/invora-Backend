using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
namespace Invora.FieldOps.Api.Middleware;
public class ApiExceptionHandler(ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        var status = exception switch
        {
            ArgumentException => 400,
            DbUpdateException { InnerException: PostgresException { SqlState: "23505" or "23503" } } => 409,
            _ => 500
        };
        logger.LogError(exception, "Request failed {Method} {Path} {StatusCode}", context.Request.Method, context.Request.Path, status);
        context.Response.StatusCode = status;
        await context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = status,
            Title = status == 400 ? "Invalid request" : status == 409 ? "Record conflicts with existing data" : "An unexpected error occurred",
            Detail = status == 400 ? exception.Message : null,
            Instance = context.Request.Path,
            Extensions = { ["traceId"] = context.TraceIdentifier }
        }, cancellationToken);
        return true;
    }
}
