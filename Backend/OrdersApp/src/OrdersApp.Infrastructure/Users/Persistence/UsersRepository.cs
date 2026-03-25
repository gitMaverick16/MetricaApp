using Microsoft.EntityFrameworkCore;
using OrdersApp.Application.Common.Interfaces;
using OrdersApp.Domain.Users;
using OrdersApp.Infrastructure.Common.Persistance;

namespace OrdersApp.Infrastructure.Users.Persistence
{
    internal sealed class UsersRepository : IUserRepository
    {
        private readonly AppDbContext _dbContext;

        public UsersRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(User user, CancellationToken cancellationToken = default)
        {
            await _dbContext.Users.AddAsync(user, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<User?> GetByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(
                u => u.Email == normalizedEmail,
                cancellationToken);
        }
    }
}
