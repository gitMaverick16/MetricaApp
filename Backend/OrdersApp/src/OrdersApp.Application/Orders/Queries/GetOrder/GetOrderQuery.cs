using ErrorOr;
using MediatR;
using OrdersApp.Domain.Orders;

namespace OrdersApp.Application.Orders.Queries.GetOrder
{
    public record GetOrderQuery
        (int Id) : IRequest<ErrorOr<Order>>;
}
