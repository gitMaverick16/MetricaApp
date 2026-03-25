using OrdersApp.Application.Common.Auth;

namespace OrdersApp.Infrastructure.Configuration
{
    public sealed class SeedUserEntry
    {
        public string Email { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
        public string Role { get; init; } = AppRoles.User;
    }
}
