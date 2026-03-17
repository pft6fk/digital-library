using Identity.Application.Features.Auth.Responses;
using Identity.Application.Services;
using Identity.Domain.Entities;
using Identity.Domain.Repositories;

namespace Identity.Application.Features.Auth.Commands;

public sealed record RegisterUserCommand(string Email, string Password, string Name);

public sealed class RegisterUserCommandHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResponse> HandleAsync(RegisterUserCommand command, CancellationToken cancellationToken = default)
    {
        var emailExists = await _userRepository.ExistsByEmailAsync(command.Email, cancellationToken);
        if (emailExists)
            throw new InvalidOperationException($"User with email '{command.Email}' already exists.");

        var passwordHash = _passwordHasher.Hash(command.Password);
        var user = User.Create(command.Email, passwordHash, command.Name);

        await _userRepository.AddAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        var token = _jwtTokenService.GenerateToken(user);
        return new AuthResponse(token, DateTime.UtcNow.AddHours(24), user.Id, user.Name, user.Email.Value);
    }
}
