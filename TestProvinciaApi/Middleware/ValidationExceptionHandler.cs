using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;

namespace TestProvinciaApi.Middleware
{
    public class ValidationExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<ValidationExceptionHandler> _logger;

        public ValidationExceptionHandler(ILogger<ValidationExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            if (exception is not ValidationException badRequestException)
            {
                return false;
            }

            _logger.LogError(
                badRequestException,
                "Exception occurred: {Message}",
                badRequestException.Message);

            var exceptionAux = (ValidationException)exception;

            var problemDetails = new HttpValidationProblemDetails(exceptionAux.Errors.GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray()))
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation Failed",
                Detail = "One or more validation errors ocurred",
                Instance = httpContext.Request.Path

            };

            httpContext.Response.StatusCode = problemDetails.Status.Value;

            await httpContext.Response
                .WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}
