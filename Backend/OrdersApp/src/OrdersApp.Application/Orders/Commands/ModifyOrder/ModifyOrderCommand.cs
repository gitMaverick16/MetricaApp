using ErrorOr;
using MediatR;
using OrdersApp.Domain.Orders;

namespace OrdersApp.Application.Orders.Commands.ModifyOrder
{
    public record ModifyOrderCommand(
        string Cliente,
        string Estado,
        DateTime Fecha,
        string NumeroPedido,
        int OrderId,
        decimal Total) : IRequest<ErrorOr<Order>>;
}
