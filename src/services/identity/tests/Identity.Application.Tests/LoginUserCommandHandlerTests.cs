using Identity.Application.Features.Auth.Commands;
using Identity.Application.Services;
using Identity.Domain.Entities;
using Identity.Domain.Repositories;
using NSubstitute;

namespace Identity.Application.Tests;

public class LoginUserCommandHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly IJwtTokenService _jwtTokenService = Substitute.For<IJwtTokenService>();
    private readonly LoginUserCommandHandler _handler;

    public LoginUserCommandHandlerTests()
    {
        _handler = new LoginUserCommandHandler(_userRepository, _passwordHasher, _jwtTokenService);
    }

    [Fact]
    public async Task HandleAsync_WithValidCredentials_ReturnsAuthResponse()
    {
        var user = User.Create("test@example.com", "hashed_pass", "John");
        _userRepository.FindByEmailAsync("test@example.com", Arg.Any<CancellationToken>()).Returns(user);
        _passwordHasher.Verify("password123", "hashed_pass").Returns(true);
        _jwtTokenService.GenerateToken(user).Returns("jwt_token");

        var command = new LoginUserCommand("test@example.com", "password123");
        var result = await _handler.HandleAsync(command);

        Assert.Equal("jwt_token", result.AccessToken);
        Assert.Equal("John", result.Name);
        Assert.Equal("test@example.com", result.Email);
    }

    [Fact]
    public async Task HandleAsync_WithWrongPassword_ThrowsUnauthorizedAccessException()
    {
        var user = User.Create("test@example.com", "hashed_pass", "John");
        _userRepository.FindByEmailAsync("test@example.com", Arg.Any<CancellationToken>()).Returns(user);
        _passwordHasher.Verify("wrong_password", "hashed_pass").Returns(false);

        var command = new LoginUserCommand("test@example.com", "wrong_password");

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _handler.HandleAsync(command));
    }

    [Fact]
    public async Task HandleAsync_WithNonExistentUser_ThrowsUnauthorizedAccessException()
    {
        _userRepository.FindByEmailAsync("nobody@example.com", Arg.Any<CancellationToken>()).Returns((User?)null);

        var command = new LoginUserCommand("nobody@example.com", "password");

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _handler.HandleAsync(command));
    }
}
