using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace TeamFlow.API;

public class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError("Unhandled exception {ExceptionType} (request {RequestId}): {StackTrace}",
            exception.GetType().FullName, httpContext.TraceIdentifier, new StackTrace(exception, fNeedFileInfo: false).ToString());

        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = exception switch
        {
            // BadRequestException => StatusCodes.Status400BadRequest,
            // NotAuthenticatedException => StatusCodes.Status401Unauthorized,
            // NotEnoughCreditException => StatusCodes.Status402PaymentRequired,
            // ForbiddenException => StatusCodes.Status403Forbidden,
            // NotFoundException => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };
        var result = await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails =
            {
                Title = "An error occurred",
                Status = httpContext.Response.StatusCode
            },
            Exception = exception
        });
        if (!result)
        {
            await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Title = "An error occurred",
                Status = httpContext.Response.StatusCode
            }, cancellationToken: cancellationToken);
        }
        return true;
    }
}
