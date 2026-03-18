using Engagement.Application.Features.Comments.Queries;
using Engagement.Domain.Entities;
using Engagement.Domain.Repositories;
using NSubstitute;

namespace Engagement.Application.Tests;

public class GetCommentsByBookIdQueryHandlerTests
{
    private readonly ICommentRepository _commentRepository = Substitute.For<ICommentRepository>();
    private readonly GetCommentsByBookIdQueryHandler _handler;

    public GetCommentsByBookIdQueryHandlerTests()
    {
        _handler = new GetCommentsByBookIdQueryHandler(_commentRepository);
    }

    [Fact]
    public async Task HandleAsync_ReturnsCommentsForBook()
    {
        var bookId = Guid.NewGuid();
        var comments = new List<Comment>
        {
            Comment.Create(bookId, Guid.NewGuid(), "User1", "Comment 1"),
            Comment.Create(bookId, Guid.NewGuid(), "User2", "Comment 2"),
        };
        _commentRepository.GetByBookIdAsync(bookId, Arg.Any<CancellationToken>())
            .Returns(comments.AsReadOnly());

        var result = await _handler.HandleAsync(bookId);

        Assert.Equal(2, result.Count);
        Assert.Equal("Comment 1", result[0].Text);
        Assert.Equal("User1", result[0].UserName);
        Assert.Equal("Comment 2", result[1].Text);
    }

    [Fact]
    public async Task HandleAsync_WithNoComments_ReturnsEmptyList()
    {
        var bookId = Guid.NewGuid();
        _commentRepository.GetByBookIdAsync(bookId, Arg.Any<CancellationToken>())
            .Returns(new List<Comment>().AsReadOnly());

        var result = await _handler.HandleAsync(bookId);

        Assert.Empty(result);
    }
}
