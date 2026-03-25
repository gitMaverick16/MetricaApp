using Microsoft.EntityFrameworkCore;
using OrdersApp.Application.Common.Interfaces;
using OrdersApp.Domain.Orders;
using OrdersApp.Infrastructure.Common.Persistance;

namespace OrdersApp.Infrastructure.Orders.Persistence
{
    public class OrdersRepository : IOrdersRepository
    {
        private readonly AppDbContext _dbContext;
        public OrdersRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddOrderAsync(Order order)
        {
            await _dbContext.Orders.AddAsync(order);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Order>> GetAllAsync()
        {
            return await _dbContext.Orders.ToListAsync();
        }

        public async Task<Order> GetByIdAsync(int id)
        {
            return await _dbContext.Orders.FirstOrDefaultAsync(order => order.Id == id);
        }

        public async Task RemoveOrderAsync(Order order)
        {
            _dbContext.Remove(order);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Order order)
        {
            _dbContext.Update(order);
            await _dbContext.SaveChangesAsync();
        }
    }
}
