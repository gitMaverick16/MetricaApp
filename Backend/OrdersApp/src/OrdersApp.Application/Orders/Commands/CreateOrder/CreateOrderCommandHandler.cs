using ErrorOr;
using MediatR;
using OrdersApp.Application.Common.Exceptions;
using OrdersApp.Application.Common.Interfaces;
using OrdersApp.Domain.Common;
using OrdersApp.Domain.Orders;

namespace OrdersApp.Application.Orders.Commands.CreateOrder
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, ErrorOr<int>>
    {
        private readonly IOrdersRepository _ordersRepository;

        public CreateOrderCommandHandler(IOrdersRepository ordersRepository)
        {
            _ordersRepository = ordersRepository;
        }

        public async Task<ErrorOr<int>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var numeroPedido = request.NumeroPedido.Trim();

            if (await _ordersRepository.ExistsByNumeroPedidoAsync(numeroPedido, cancellationToken))
            {
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
                return Error.Validation(description: ex.Message);
            }

            try
            {
                await _ordersRepository.AddOrderAsync(order, cancellationToken);
            }
            catch (DuplicateNumeroPedidoException)
            {
                return Error.Conflict(description: "Ya existe un pedido con ese número.");
            }

            return order.Id;
        }
    }
}
