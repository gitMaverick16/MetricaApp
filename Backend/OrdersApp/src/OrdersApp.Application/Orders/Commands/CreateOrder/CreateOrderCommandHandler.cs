using ErrorOr;
using MediatR;
using OrdersApp.Application.Common.Interfaces;
using OrdersApp.Domain.Orders;

namespace OrdersApp.Application.Orders.Commands.CreateOrder
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, ErrorOr<int>>
    {
        private readonly IOrdersRepository _ordersRepository;
        //private readonly IUnitOfWork _unitOfWork;
        public CreateOrderCommandHandler(
            IOrdersRepository ordersRepository)
            //IUnitOfWork unitOfWork)
        {
            _ordersRepository = ordersRepository;
            //_unitOfWork = unitOfWork;
        }
        public async Task<ErrorOr<int>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = new Order
            {
                Cliente = request.Cliente,
                Estado = request.Estado,
                Fecha = request.Fecha,
                NumeroPedido = request.NumeroPedido,
                Total = request.Total
            };

            await _ordersRepository.AddOrderAsync(order);

            //await _unitOfWork.CommitChangesAsync();

            return 1;
        }
    }
}
