using OrdersApp.Domain.Orders;

namespace OrdersApp.Application.Common.Interfaces
{
    public interface IOrdersRepository
    {
        Task AddOrderAsync(Order order);
        Task<List<Order>> GetAllAsync();
        Task<Order> GetByIdAsync(int id);
        Task RemoveOrderAsync(Order order);
        Task UpdateAsync(Order order);
    }
}
