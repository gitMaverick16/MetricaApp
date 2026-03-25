using Microsoft.EntityFrameworkCore;
using OrdersApp.Domain.Orders;

namespace OrdersApp.Infrastructure.Common.Persistance
{
    public class AppDbContext : DbContext
    {
        public DbSet<Order> Orders { get; set; } = null!;

        public AppDbContext(DbContextOptions options) : base(options)
        {
            
        }
    }
}
