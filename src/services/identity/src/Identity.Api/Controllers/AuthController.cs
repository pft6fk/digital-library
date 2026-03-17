using System.Security.Claims;
using Identity.Application.Features.Auth.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly RegisterUserCommandHandler _registerHandler;
    private readonly LoginUserCommandHandler _loginHandler;

    public AuthController(
        RegisterUserCommandHandler registerHandler,
        LoginUserCommandHandler loginHandler)
    {
        _registerHandler = registerHandler;
        _loginHandler = loginHandler;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand(request.Email, request.Password, request.Name);
        var result = await _registerHandler.HandleAsync(command, cancellationToken);
        return StatusCode(201, result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var command = new LoginUserCommand(request.Email, request.Password);
        var result = await _loginHandler.HandleAsync(command, cancellationToken);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? User.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub);
        var email = User.FindFirstValue(ClaimTypes.Email)
                    ?? User.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email);
        var name = User.FindFirstValue(ClaimTypes.Name);

        return Ok(new { UserId = userId, Email = email, Name = name });
    }
}

public sealed record RegisterRequest(string Email, string Password, string Name);
public sealed record LoginRequest(string Email, string Password);
