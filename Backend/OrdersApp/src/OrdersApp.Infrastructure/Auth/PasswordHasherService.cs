using Microsoft.AspNetCore.Identity;
using OrdersApp.Application.Common.Interfaces;

namespace OrdersApp.Infrastructure.Auth
{
    internal sealed class PasswordHasherService : IPasswordHasher
    {
        private static readonly PasswordHasher<MarkerUser> Hasher = new();
        private static readonly MarkerUser Marker = new();

        public string Hash(string plainPassword)
        {
            return Hasher.HashPassword(Marker, plainPassword);
        }

        public bool Verify(string plainPassword, string passwordHash)
        {
            var result = Hasher.VerifyHashedPassword(Marker, passwordHash, plainPassword);
            return result is PasswordVerificationResult.Success
                or PasswordVerificationResult.SuccessRehashNeeded;
        }

        private sealed class MarkerUser
        {
        }
    }
}
