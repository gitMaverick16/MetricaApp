using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OrdersApp.Application.Common.Exceptions;
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

        public async Task AddOrderAsync(Order order, CancellationToken cancellationToken = default)
        {
            await _dbContext.Orders.AddAsync(order, cancellationToken);
            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
            {
                throw new DuplicateNumeroPedidoException(ex);
            }
        }

        public async Task<bool> ExistsByNumeroPedidoAsync(string numeroPedido, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Orders.IgnoreQueryFilters().AnyAsync(
                o => o.NumeroPedido == numeroPedido,
                cancellationToken);
        }

        public async Task<bool> ExistsByNumeroPedidoExcludingIdAsync(
            string numeroPedido,
            int excludeOrderId,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.Orders.IgnoreQueryFilters().AnyAsync(
                o => o.NumeroPedido == numeroPedido && o.Id != excludeOrderId,
                cancellationToken);
        }

        public async Task<List<Order>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Orders.ToListAsync(cancellationToken);
        }

        public async Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Orders.FirstOrDefaultAsync(order => order.Id == id, cancellationToken);
        }

        public async Task UpdateAsync(Order order, CancellationToken cancellationToken = default)
        {
            _dbContext.Update(order);
            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
            {
                throw new DuplicateNumeroPedidoException(ex);
            }
        }

        private static bool IsUniqueConstraintViolation(DbUpdateException ex)
        {
            return ex.InnerException is SqlException sql && (sql.Number == 2601 || sql.Number == 2627);
        }
    }
}
