using Identity.Application.Features.Auth.Responses;
using Identity.Application.Services;
using Identity.Domain.Repositories;

namespace Identity.Application.Features.Auth.Commands;

public sealed record LoginUserCommand(string Email, string Password);

public sealed class LoginUserCommandHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public LoginUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResponse> HandleAsync(LoginUserCommand command, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.FindByEmailAsync(command.Email, cancellationToken);
        if (user is null || !_passwordHasher.Verify(command.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password.");

        var token = _jwtTokenService.GenerateToken(user);
        return new AuthResponse(token, DateTime.UtcNow.AddHours(24), user.Id, user.Name, user.Email.Value);
    }
}
