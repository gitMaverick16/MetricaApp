using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using OrdersApp.Application.Common.Exceptions;
using OrdersApp.Application.Common.Interfaces;
using OrdersApp.Domain.Common;
using OrdersApp.Domain.Orders;

namespace OrdersApp.Application.Orders.Commands.ModifyOrder
{
    public class ModifyOrderCommandHandler : IRequestHandler<ModifyOrderCommand, ErrorOr<Order>>
    {
        private readonly IOrdersRepository _ordersRepository;
        private readonly ILogger<ModifyOrderCommandHandler> _logger;

        public ModifyOrderCommandHandler(
            IOrdersRepository ordersRepository,
            ILogger<ModifyOrderCommandHandler> logger)
        {
            _ordersRepository = ordersRepository;
            _logger = logger;
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
                _logger.LogInformation(
                    "Actualización rechazada: número de pedido ya en uso {NumeroPedido} {OrderId}",
                    numeroPedido,
                    command.OrderId);
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
                _logger.LogInformation(
                    ex,
                    "Validación de dominio al actualizar pedido {OrderId} {NumeroPedido}",
                    command.OrderId,
                    numeroPedido);
                return Error.Validation(description: ex.Message);
            }

            try
            {
                await _ordersRepository.UpdateAsync(order, cancellationToken);
            }
            catch (DuplicateNumeroPedidoException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Conflicto de unicidad al actualizar pedido {OrderId} {NumeroPedido}",
                    command.OrderId,
                    numeroPedido);
                return Error.Conflict(description: "Ya existe un pedido con ese número.");
            }

            _logger.LogInformation(
                "Pedido actualizado {OrderId} {NumeroPedido}",
                order.Id,
                order.NumeroPedido);

            return order;
        }
    }
}
