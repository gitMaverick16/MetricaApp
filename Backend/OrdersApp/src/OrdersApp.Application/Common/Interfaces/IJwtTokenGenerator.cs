namespace OrdersApp.Application.Common.Interfaces
{
    public interface IJwtTokenGenerator
    {
        JwtTokenResult CreateToken(Guid userId, string email, string role);
    }

    public sealed record JwtTokenResult(string Token, int ExpiresInSeconds);
}
