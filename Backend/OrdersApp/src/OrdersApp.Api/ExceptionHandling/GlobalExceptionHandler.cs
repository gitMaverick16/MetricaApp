using Microsoft.AspNetCore.Diagnostics;
using OrdersApp.Api.Common;

namespace OrdersApp.Api.ExceptionHandling
{
    public sealed class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly IHostEnvironment _environment;
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(IHostEnvironment environment, ILogger<GlobalExceptionHandler> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var (statusCode, detail) = HttpErrorMapper.FromException(exception, _environment);

            if (statusCode >= 500)
            {
                _logger.LogError(exception, "Excepción no controlada");
            }
            else
            {
                _logger.LogWarning(exception, "Excepción de solicitud");
            }

            await Results
                .Problem(detail: detail, statusCode: statusCode)
                .ExecuteAsync(httpContext);

            return true;
        }
    }
}
