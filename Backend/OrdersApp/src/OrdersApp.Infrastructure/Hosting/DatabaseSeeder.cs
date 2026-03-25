using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrdersApp.Application.Common.Interfaces;
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

            if (!seedOptions.Enabled || seedOptions.Users.Count == 0)
            {
                return;
            }

            var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

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
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
