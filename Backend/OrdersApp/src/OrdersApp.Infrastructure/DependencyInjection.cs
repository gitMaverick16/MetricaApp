using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using OrdersApp.Application.Common.Interfaces;
using OrdersApp.Infrastructure.Auth;
using OrdersApp.Infrastructure.Common.Persistance;
using OrdersApp.Infrastructure.Configuration;
using OrdersApp.Infrastructure.Hosting;
using OrdersApp.Infrastructure.Orders.Persistence;
using OrdersApp.Infrastructure.Users.Persistence;

namespace OrdersApp.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("OrdersAppDatabase");
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            var jwtSection = configuration.GetSection(JwtSettings.SectionName);
            var jwtSettings = jwtSection.Get<JwtSettings>()
                ?? throw new InvalidOperationException($"La sección {JwtSettings.SectionName} no está configurada.");

            services.Configure<JwtSettings>(jwtSection);
            services.Configure<SeedSettings>(configuration.GetSection(SeedSettings.SectionName));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            services.AddAuthorization();

            services.AddScoped<IOrdersRepository, OrdersRepository>();
            services.AddScoped<IUserRepository, UsersRepository>();
            services.AddSingleton<IPasswordHasher, PasswordHasherService>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

            services.AddHostedService<DatabaseSeeder>();

            return services;
        }
    }
}
