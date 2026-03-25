using ErrorOr;
using MediatR;

namespace OrdersApp.Application.Orders.Commands.CreateOrder
{
    public record CreateOrderCommand(
        string Cliente,
        string Estado,
        DateTime Fecha,
        string NumeroPedido,
        decimal Total) : IRequest<ErrorOr<int>>;
}