using ErrorOr;
using MediatR;
using OrdersApp.Application.Common.Interfaces;
using System.Security;

namespace OrdersApp.Application.Orders.Commands.DeleteOrder
{
    public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand, ErrorOr<Deleted>>
    {
        private readonly IOrdersRepository _ordersRepository;
        public DeleteOrderCommandHandler(IOrdersRepository ordersRepository)
        {
            _ordersRepository = ordersRepository;
        }
        public async Task<ErrorOr<Deleted>> Handle(DeleteOrderCommand command, CancellationToken cancellationToken)
        {
            var order = await _ordersRepository.GetByIdAsync(command.Id);

            if (order is null)
            {
                return Error.NotFound(description: "Order not found");
            }

            await _ordersRepository.RemoveOrderAsync(order);
            return Result.Deleted;
        }
    }
}
