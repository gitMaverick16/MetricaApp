using ErrorOr;
using MediatR;
using OrdersApp.Domain.Orders;

namespace OrdersApp.Application.Orders.Queries.GetAllOrders
{
    public record GetAllOrdersQuery : IRequest<ErrorOr<List<Order>>>;
}
