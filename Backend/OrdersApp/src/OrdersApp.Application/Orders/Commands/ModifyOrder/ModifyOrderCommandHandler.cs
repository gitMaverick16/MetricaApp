using ErrorOr;
using MediatR;
using OrdersApp.Application.Common.Exceptions;
using OrdersApp.Application.Common.Interfaces;
using OrdersApp.Domain.Common;
using OrdersApp.Domain.Orders;

namespace OrdersApp.Application.Orders.Commands.ModifyOrder
{
    public class ModifyOrderCommandHandler : IRequestHandler<ModifyOrderCommand, ErrorOr<Order>>
    {
        private readonly IOrdersRepository _ordersRepository;

        public ModifyOrderCommandHandler(IOrdersRepository ordersRepository)
        {
            _ordersRepository = ordersRepository;
        }

        public async Task<ErrorOr<Order>> Handle(ModifyOrderCommand command, CancellationToken cancellationToken)
        {
            var order = await _ordersRepository.GetByIdAsync(command.OrderId, cancellationToken);

            if (order is null)
            {
                return Error.NotFound(description: "Order not found");
            }

            var numeroPedido = command.NumeroPedido.Trim();

            if (await _ordersRepository.ExistsByNumeroPedidoExcludingIdAsync(
                    numeroPedido,
                    command.OrderId,
                    cancellationToken))
            {
                return Error.Conflict(description: "Ya existe un pedido con ese número.");
            }

            try
            {
                order.UpdateDetails(
                    numeroPedido,
                    command.Cliente,
                    command.Estado,
                    command.Fecha,
                    command.Total);
            }
            catch (OrderDomainException ex)
            {
                return Error.Validation(description: ex.Message);
            }

            try
            {
                await _ordersRepository.UpdateAsync(order, cancellationToken);
            }
            catch (DuplicateNumeroPedidoException)
            {
                return Error.Conflict(description: "Ya existe un pedido con ese número.");
            }

            return order;
        }
    }
}
