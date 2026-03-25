namespace OrdersApp.Api.Contracts
{
    public sealed class LoginResponse
    {
        public required string Token { get; init; }
        public required int ExpiresIn { get; init; }
    }
}
