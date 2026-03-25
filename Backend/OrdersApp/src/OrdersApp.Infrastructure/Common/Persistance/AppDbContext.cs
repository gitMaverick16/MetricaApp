using Microsoft.EntityFrameworkCore;
using OrdersApp.Domain.Orders;
using OrdersApp.Domain.Users;
using OrdersApp.Infrastructure.Users.Persistence;

namespace OrdersApp.Infrastructure.Common.Persistance
{
    public class AppDbContext : DbContext
    {
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;

        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasQueryFilter(e => !e.IsDeleted);
                entity.HasIndex(e => e.NumeroPedido).IsUnique();
                entity.Property(e => e.NumeroPedido).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Cliente).HasMaxLength(150).IsRequired();
                entity.Property(e => e.Estado).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Total).HasColumnType("decimal(10,2)");
                entity.Property(e => e.IsDeleted).IsRequired();
            });
        }
    }
}
