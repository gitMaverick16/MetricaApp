namespace OrdersApp.Domain.Users
{
    public sealed class User
    {
        public Guid Id { get; private set; }
        public string Email { get; private set; } = null!;
        public string PasswordHash { get; private set; } = null!;
        public string Role { get; private set; } = null!;

        private User()
        {
        }

        public static User Create(string email, string passwordHash, string role)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(email);
            ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);
            ArgumentException.ThrowIfNullOrWhiteSpace(role);

            return new User
            {
                Id = Guid.NewGuid(),
                Email = NormalizeEmail(email),
                PasswordHash = passwordHash,
                Role = role.Trim()
            };
        }

        public static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();
    }
}
