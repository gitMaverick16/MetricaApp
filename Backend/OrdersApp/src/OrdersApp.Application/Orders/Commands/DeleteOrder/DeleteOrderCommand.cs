using ErrorOr;
using MediatR;

namespace OrdersApp.Application.Orders.Commands.DeleteOrder
{
    public record DeleteOrderCommand(int Id) : IRequest<ErrorOr<Deleted>>;
}
