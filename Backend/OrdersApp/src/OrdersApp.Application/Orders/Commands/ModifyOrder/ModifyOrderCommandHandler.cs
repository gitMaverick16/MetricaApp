using ErrorOr;
using MediatR;
using OrdersApp.Application.Common.Interfaces;
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
            var order = await _ordersRepository.GetByIdAsync(command.OrderId);

            if (order == null)
            {
                return Error.NotFound(description: "Order not found");
            }

            order.NumeroPedido = command.NumeroPedido;
            order.Cliente = command.Cliente;
            order.Estado = command.Estado;
            order.Total = command.Total;
            order.Fecha = command.Fecha;

            await _ordersRepository.UpdateAsync(order);

            return order;
        }
    }
}
