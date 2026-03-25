using OrdersApp.Domain.Orders;

namespace OrdersApp.Application.Common.Interfaces
{
    public interface IOrdersRepository
    {
        Task AddOrderAsync(Order order, CancellationToken cancellationToken = default);
        Task<bool> ExistsByNumeroPedidoAsync(string numeroPedido, CancellationToken cancellationToken = default);
        Task<bool> ExistsByNumeroPedidoExcludingIdAsync(
            string numeroPedido,
            int excludeOrderId,
            CancellationToken cancellationToken = default);
        Task<List<Order>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task UpdateAsync(Order order, CancellationToken cancellationToken = default);
    }
}
