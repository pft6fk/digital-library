using Engagement.Domain.Entities;

namespace Engagement.Domain.Tests;

public class CommentTests
{
    [Fact]
    public void Create_WithValidData_ReturnsComment()
    {
        var bookId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var comment = Comment.Create(bookId, userId, "John", "Great book!");

        Assert.NotEqual(Guid.Empty, comment.Id);
        Assert.Equal(bookId, comment.BookId);
        Assert.Equal(userId, comment.UserId);
        Assert.Equal("John", comment.UserName);
        Assert.Equal("Great book!", comment.Text);
        Assert.True(comment.CreatedAt <= DateTime.UtcNow);
        Assert.True(comment.CreatedAt > DateTime.UtcNow.AddSeconds(-5));
    }

    [Fact]
    public void Create_TrimsText()
    {
        var comment = Comment.Create(Guid.NewGuid(), Guid.NewGuid(), "User", "  Nice  ");

        Assert.Equal("Nice", comment.Text);
    }

    [Fact]
    public void Create_WithEmptyBookId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            Comment.Create(Guid.Empty, Guid.NewGuid(), "User", "Text"));
    }

    [Fact]
    public void Create_WithEmptyUserId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            Comment.Create(Guid.NewGuid(), Guid.Empty, "User", "Text"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithEmptyText_ThrowsArgumentException(string? text)
    {
        Assert.Throws<ArgumentException>(() =>
            Comment.Create(Guid.NewGuid(), Guid.NewGuid(), "User", text!));
    }

    [Fact]
    public void Create_GeneratesUniqueIds()
    {
        var c1 = Comment.Create(Guid.NewGuid(), Guid.NewGuid(), "User", "Text 1");
        var c2 = Comment.Create(Guid.NewGuid(), Guid.NewGuid(), "User", "Text 2");

        Assert.NotEqual(c1.Id, c2.Id);
    }
}
