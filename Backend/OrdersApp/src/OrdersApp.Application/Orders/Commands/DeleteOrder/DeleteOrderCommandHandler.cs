using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using OrdersApp.Application.Common.Interfaces;

namespace OrdersApp.Application.Orders.Commands.DeleteOrder
{
    public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand, ErrorOr<Deleted>>
    {
        private readonly IOrdersRepository _ordersRepository;
        private readonly ILogger<DeleteOrderCommandHandler> _logger;

        public DeleteOrderCommandHandler(
            IOrdersRepository ordersRepository,
            ILogger<DeleteOrderCommandHandler> logger)
        {
            _ordersRepository = ordersRepository;
            _logger = logger;
        }
        public async Task<ErrorOr<Deleted>> Handle(DeleteOrderCommand command, CancellationToken cancellationToken)
        {
            var order = await _ordersRepository.GetByIdAsync(command.Id, cancellationToken);

            if (order is null)
            {
                return Error.NotFound(description: "Order not found");
            }

            order.MarkAsDeleted();
            await _ordersRepository.UpdateAsync(order, cancellationToken);

            _logger.LogInformation(
                "Pedido eliminado lógicamente {OrderId} {NumeroPedido}",
                order.Id,
                order.NumeroPedido);

            return Result.Deleted;
        }
    }
}
