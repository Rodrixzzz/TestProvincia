using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TestProvincia.Shared.Exceptions;

namespace TestProvinciaApi.Middleware
{
    public class DuplicatedExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<DuplicatedExceptionHandler> _logger;

        public DuplicatedExceptionHandler(ILogger<DuplicatedExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            if (exception is not DuplicatedException duplicatedException)
            {
                return false;
            }

            _logger.LogError(
                duplicatedException,
                "Exception occurred: {Message}",
                duplicatedException.Message);

            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Duplicated entity",
                Detail = duplicatedException.Message,
                Instance = httpContext.Request.Path
            };

            httpContext.Response.StatusCode = problemDetails.Status.Value;

            await httpContext.Response
                .WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}
