using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrdersApp.Api.Common;
using OrdersApp.Api.Contracts;
using OrdersApp.Application.Auth.Commands.Login;

namespace OrdersApp.Api.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var command = new LoginCommand(request.Email, request.Password);
            var result = await _mediator.Send(command);

            if (result.IsError)
            {
                return ProblemFromError(result.FirstError);
            }

            var value = result.Value;
            return Ok(new LoginResponse
            {
                Token = value.Token,
                ExpiresIn = value.ExpiresIn
            });
        }

        private IActionResult ProblemFromError(Error error)
        {
            var (statusCode, detail) = HttpErrorMapper.FromError(error);
            return Problem(detail: detail, statusCode: statusCode);
        }
    }
}
