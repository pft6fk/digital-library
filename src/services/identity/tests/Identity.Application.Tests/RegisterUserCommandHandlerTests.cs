using Identity.Application.Features.Auth.Commands;
using Identity.Application.Services;
using Identity.Domain.Entities;
using Identity.Domain.Repositories;
using NSubstitute;

namespace Identity.Application.Tests;

public class RegisterUserCommandHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly IJwtTokenService _jwtTokenService = Substitute.For<IJwtTokenService>();
    private readonly RegisterUserCommandHandler _handler;

    public RegisterUserCommandHandlerTests()
    {
        _handler = new RegisterUserCommandHandler(_userRepository, _passwordHasher, _jwtTokenService);
    }

    [Fact]
    public async Task HandleAsync_WithValidData_ReturnsAuthResponse()
    {
        _userRepository.ExistsByEmailAsync("test@example.com", Arg.Any<CancellationToken>()).Returns(false);
        _passwordHasher.Hash("password123").Returns("hashed_password");
        _jwtTokenService.GenerateToken(Arg.Any<User>()).Returns("jwt_token");

        var command = new RegisterUserCommand("test@example.com", "password123", "John Doe");
        var result = await _handler.HandleAsync(command);

        Assert.Equal("jwt_token", result.AccessToken);
        Assert.Equal("John Doe", result.Name);
        Assert.Equal("test@example.com", result.Email);
        await _userRepository.Received(1).AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
        await _userRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WithDuplicateEmail_ThrowsInvalidOperationException()
    {
        _userRepository.ExistsByEmailAsync("existing@example.com", Arg.Any<CancellationToken>()).Returns(true);

        var command = new RegisterUserCommand("existing@example.com", "password123", "Jane");

        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.HandleAsync(command));
        await _userRepository.DidNotReceive().AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_HashesPassword()
    {
        _userRepository.ExistsByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);
        _passwordHasher.Hash("myPassword").Returns("hashed_myPassword");
        _jwtTokenService.GenerateToken(Arg.Any<User>()).Returns("token");

        var command = new RegisterUserCommand("test@example.com", "myPassword", "User");
        await _handler.HandleAsync(command);

        _passwordHasher.Received(1).Hash("myPassword");
    }
}
