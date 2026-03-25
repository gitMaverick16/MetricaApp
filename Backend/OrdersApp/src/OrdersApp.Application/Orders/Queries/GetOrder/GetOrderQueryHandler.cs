using ErrorOr;
using MediatR;
using OrdersApp.Application.Common.Interfaces;
using OrdersApp.Domain.Orders;

namespace OrdersApp.Application.Orders.Queries.GetOrder
{
    public class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, ErrorOr<Order>>
    {
        private readonly IOrdersRepository _ordersRepository;
        public GetOrderQueryHandler(IOrdersRepository ordersRepository)
        {
            _ordersRepository = ordersRepository;
        }
        public async Task<ErrorOr<Order>> Handle(GetOrderQuery request, CancellationToken cancellationToken)
        {
            var order = await _ordersRepository.GetByIdAsync(request.Id, cancellationToken);
            return order is null
                ? Error.NotFound(description: "Order not found")
                : order;
        }
    }
}
