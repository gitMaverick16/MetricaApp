using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using OrdersApp.Application.Common.Exceptions;
using OrdersApp.Application.Common.Interfaces;
using OrdersApp.Domain.Common;
using OrdersApp.Domain.Orders;

namespace OrdersApp.Application.Orders.Commands.CreateOrder
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, ErrorOr<int>>
    {
        private readonly IOrdersRepository _ordersRepository;
        private readonly ILogger<CreateOrderCommandHandler> _logger;

        public CreateOrderCommandHandler(
            IOrdersRepository ordersRepository,
            ILogger<CreateOrderCommandHandler> logger)
        {
            _ordersRepository = ordersRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<int>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var numeroPedido = request.NumeroPedido.Trim();

            if (await _ordersRepository.ExistsByNumeroPedidoAsync(numeroPedido, cancellationToken))
            {
                _logger.LogInformation(
                    "Creación rechazada: número de pedido ya en uso {NumeroPedido}",
                    numeroPedido);
                return Error.Conflict(description: "Ya existe un pedido con ese número.");
            }

            Order order;
            try
            {
                order = Order.Create(
                    numeroPedido,
                    request.Cliente,
                    request.Estado,
                    request.Fecha,
                    request.Total);
            }
            catch (OrderDomainException ex)
            {
                _logger.LogInformation(
                    ex,
                    "Validación de dominio al crear pedido {NumeroPedido}",
                    numeroPedido);
                return Error.Validation(description: ex.Message);
            }

            try
            {
                await _ordersRepository.AddOrderAsync(order, cancellationToken);
            }
            catch (DuplicateNumeroPedidoException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Conflicto de unicidad al persistir pedido {NumeroPedido}",
                    numeroPedido);
                return Error.Conflict(description: "Ya existe un pedido con ese número.");
            }

            _logger.LogInformation(
                "Pedido creado {OrderId} {NumeroPedido}",
                order.Id,
                numeroPedido);

            return order.Id;
        }
    }
}
