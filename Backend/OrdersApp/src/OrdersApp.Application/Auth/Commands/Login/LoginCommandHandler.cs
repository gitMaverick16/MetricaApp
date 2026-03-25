using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using OrdersApp.Application.Common.Interfaces;
using OrdersApp.Domain.Users;

namespace OrdersApp.Application.Auth.Commands.Login
{
    public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, ErrorOr<LoginResult>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly ILogger<LoginCommandHandler> _logger;

        public LoginCommandHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator jwtTokenGenerator,
            ILogger<LoginCommandHandler> logger)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
            _logger = logger;
        }

        public async Task<ErrorOr<LoginResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return Error.Validation(description: "Email y contraseña son obligatorios.");
            }

            var normalizedEmail = User.NormalizeEmail(request.Email);
            var user = await _userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);

            if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            {
                _logger.LogInformation("Intento de login fallido para {Email}", normalizedEmail);
                return Error.Unauthorized(description: "Credenciales inválidas.");
            }

            var tokenResult = _jwtTokenGenerator.CreateToken(user.Id, user.Email, user.Role);

            _logger.LogInformation("Usuario autenticado {UserId} {Email}", user.Id, user.Email);

            return new LoginResult(tokenResult.Token, tokenResult.ExpiresInSeconds);
        }
    }
}
