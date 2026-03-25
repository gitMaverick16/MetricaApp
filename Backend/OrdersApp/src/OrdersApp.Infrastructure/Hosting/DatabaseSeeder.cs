using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrdersApp.Application.Common.Interfaces;
using OrdersApp.Domain.Common;
using OrdersApp.Domain.Orders;
using OrdersApp.Domain.Users;
using OrdersApp.Infrastructure.Common.Persistance;
using OrdersApp.Infrastructure.Configuration;

namespace OrdersApp.Infrastructure.Hosting
{
    internal sealed class DatabaseSeeder : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DatabaseSeeder> _logger;

        public DatabaseSeeder(IServiceProvider serviceProvider, ILogger<DatabaseSeeder> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var seedOptions = scope.ServiceProvider.GetRequiredService<IOptions<SeedSettings>>().Value;

            await db.Database.MigrateAsync(cancellationToken);
            _logger.LogInformation("Migraciones de base de datos comprobadas o aplicadas.");

            if (!seedOptions.Enabled)
            {
                return;
            }

            var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
            var ordersRepository = scope.ServiceProvider.GetRequiredService<IOrdersRepository>();

            foreach (var entry in seedOptions.Users)
            {
                if (string.IsNullOrWhiteSpace(entry.Email) || string.IsNullOrWhiteSpace(entry.Password))
                {
                    continue;
                }

                var normalized = User.NormalizeEmail(entry.Email);
                if (await db.Users.AnyAsync(u => u.Email == normalized, cancellationToken))
                {
                    continue;
                }

                var hash = passwordHasher.Hash(entry.Password);
                var user = User.Create(entry.Email, hash, entry.Role);
                await userRepository.AddAsync(user, cancellationToken);

                _logger.LogInformation("Usuario de seed creado: {Email} ({Role})", user.Email, user.Role);
            }

            foreach (var orderEntry in seedOptions.Orders)
            {
                if (string.IsNullOrWhiteSpace(orderEntry.NumeroPedido))
                {
                    continue;
                }

                if (await ordersRepository.ExistsByNumeroPedidoAsync(orderEntry.NumeroPedido, cancellationToken))
                {
                    continue;
                }

                Order order;
                try
                {
                    order = Order.Create(
                        orderEntry.NumeroPedido,
                        orderEntry.Cliente,
                        orderEntry.Estado,
                        orderEntry.Fecha,
                        orderEntry.Total);
                }
                catch (OrderDomainException ex)
                {
                    _logger.LogWarning(ex, "Seed de pedido omitido: datos inválidos para {NumeroPedido}", orderEntry.NumeroPedido);
                    continue;
                }

                await ordersRepository.AddOrderAsync(order, cancellationToken);
                _logger.LogInformation("Pedido de seed creado: {NumeroPedido}", order.NumeroPedido);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
