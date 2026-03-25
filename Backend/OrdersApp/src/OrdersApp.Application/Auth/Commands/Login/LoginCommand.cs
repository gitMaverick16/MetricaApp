using ErrorOr;
using MediatR;

namespace OrdersApp.Application.Auth.Commands.Login
{
    public sealed record LoginCommand(string Email, string Password) : IRequest<ErrorOr<LoginResult>>;

    public sealed record LoginResult(string Token, int ExpiresIn);
}
