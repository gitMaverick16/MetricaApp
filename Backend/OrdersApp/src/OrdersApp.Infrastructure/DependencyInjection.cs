using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrdersApp.Application.Common.Interfaces;
using OrdersApp.Infrastructure.Common.Persistance;
using OrdersApp.Infrastructure.Orders.Persistence;

namespace OrdersApp.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("OrdersAppDatabase");
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));
            services.AddScoped<IOrdersRepository, OrdersRepository>();
            return services;
        }
    }
}
