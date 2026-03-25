using ErrorOr;
using MediatR;
using OrdersApp.Application.Common.Interfaces;
using OrdersApp.Domain.Orders;

namespace OrdersApp.Application.Orders.Queries.GetAllOrders
{
    public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, ErrorOr<List<Order>>>
    {
        private readonly IOrdersRepository _ordersRepository;
        public GetAllOrdersQueryHandler(IOrdersRepository ordersRepository)
        {
            _ordersRepository = ordersRepository;
        }
        public async Task<ErrorOr<List<Order>>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
        {
            var orders = await _ordersRepository.GetAllAsync();
            return orders;
        }
    }
}
