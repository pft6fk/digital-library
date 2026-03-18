using Catalog.Application.Features.Authors.Commands;
using Catalog.Domain.Entities;
using Catalog.Domain.Repositories;
using NSubstitute;

namespace Catalog.Application.Tests;

public class CreateAuthorCommandHandlerTests
{
    private readonly IAuthorRepository _authorRepository = Substitute.For<IAuthorRepository>();
    private readonly CreateAuthorCommandHandler _handler;

    public CreateAuthorCommandHandlerTests()
    {
        _handler = new CreateAuthorCommandHandler(_authorRepository);
    }

    [Fact]
    public async Task HandleAsync_WithValidData_ReturnsAuthorId()
    {
        var command = new CreateAuthorCommand("Leo Tolstoy", "Russian novelist", new DateTime(1828, 9, 9));
        var result = await _handler.HandleAsync(command);

        Assert.NotEqual(Guid.Empty, result);
        await _authorRepository.Received(1).AddAsync(Arg.Any<Author>(), Arg.Any<CancellationToken>());
        await _authorRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WithEmptyName_ThrowsArgumentException()
    {
        var command = new CreateAuthorCommand("", null, null);

        await Assert.ThrowsAsync<ArgumentException>(() => _handler.HandleAsync(command));
        await _authorRepository.DidNotReceive().AddAsync(Arg.Any<Author>(), Arg.Any<CancellationToken>());
    }
}
