using ErrorOr;
using OrdersApp.Application.Common.Exceptions;
using OrdersApp.Domain.Common;

namespace OrdersApp.Api.Common
{
    public static class HttpErrorMapper
    {
        public static (int StatusCode, string Detail) FromError(Error error)
        {
            return error.Type switch
            {
                ErrorType.Validation => (400, error.Description),
                ErrorType.Conflict => (409, error.Description),
                ErrorType.NotFound => (404, error.Description),
                ErrorType.Unauthorized => (401, error.Description),
                _ => (500, error.Description)
            };
        }

        public static (int StatusCode, string Detail) FromException(Exception exception, IHostEnvironment environment)
        {
            switch (exception)
            {
                case OrderDomainException domainEx:
                    return (400, domainEx.Message);
                case DuplicateNumeroPedidoException dupEx:
                    return (409, dupEx.Message);
                case ArgumentException or ArgumentNullException:
                    return (400, exception.Message);
                default:
                    if (environment.IsDevelopment())
                    {
                        return (500, exception.Message);
                    }

                    return (500, "Se produjo un error interno.");
            }
        }
    }
}
