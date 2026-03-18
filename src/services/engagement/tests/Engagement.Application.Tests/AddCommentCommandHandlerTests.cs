using Engagement.Application.Features.Comments.Commands;
using Engagement.Domain.Entities;
using Engagement.Domain.Repositories;
using NSubstitute;

namespace Engagement.Application.Tests;

public class AddCommentCommandHandlerTests
{
    private readonly ICommentRepository _commentRepository = Substitute.For<ICommentRepository>();
    private readonly AddCommentCommandHandler _handler;

    public AddCommentCommandHandlerTests()
    {
        _handler = new AddCommentCommandHandler(_commentRepository);
    }

    [Fact]
    public async Task HandleAsync_WithValidData_ReturnsCommentId()
    {
        var command = new AddCommentCommand(Guid.NewGuid(), Guid.NewGuid(), "John", "Great book!");
        var result = await _handler.HandleAsync(command);

        Assert.NotEqual(Guid.Empty, result);
        await _commentRepository.Received(1).AddAsync(Arg.Any<Comment>(), Arg.Any<CancellationToken>());
        await _commentRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WithEmptyText_ThrowsArgumentException()
    {
        var command = new AddCommentCommand(Guid.NewGuid(), Guid.NewGuid(), "User", "");

        await Assert.ThrowsAsync<ArgumentException>(() => _handler.HandleAsync(command));
        await _commentRepository.DidNotReceive().AddAsync(Arg.Any<Comment>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WithEmptyBookId_ThrowsArgumentException()
    {
        var command = new AddCommentCommand(Guid.Empty, Guid.NewGuid(), "User", "Text");

        await Assert.ThrowsAsync<ArgumentException>(() => _handler.HandleAsync(command));
    }
}
