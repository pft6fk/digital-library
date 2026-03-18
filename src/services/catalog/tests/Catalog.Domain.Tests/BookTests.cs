using Catalog.Domain.Entities;

namespace Catalog.Domain.Tests;

public class BookTests
{
    [Fact]
    public void Create_WithValidData_ReturnsBook()
    {
        var authorId = Guid.NewGuid();
        var book = Book.Create("War and Peace", "Epic novel", authorId);

        Assert.NotEqual(Guid.Empty, book.Id);
        Assert.Equal("War and Peace", book.Title);
        Assert.Equal("Epic novel", book.Description);
        Assert.Equal(authorId, book.AuthorId);
    }

    [Fact]
    public void Create_WithNullDescription_ReturnsBook()
    {
        var authorId = Guid.NewGuid();
        var book = Book.Create("Title", null, authorId);

        Assert.Null(book.Description);
    }

    [Fact]
    public void Create_TrimsTitle()
    {
        var book = Book.Create("  War and Peace  ", null, Guid.NewGuid());

        Assert.Equal("War and Peace", book.Title);
    }

    [Fact]
    public void Create_TrimsDescription()
    {
        var book = Book.Create("Title", "  A description  ", Guid.NewGuid());

        Assert.Equal("A description", book.Description);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithEmptyTitle_ThrowsArgumentException(string? title)
    {
        Assert.Throws<ArgumentException>(() => Book.Create(title!, null, Guid.NewGuid()));
    }

    [Fact]
    public void Create_WithEmptyAuthorId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => Book.Create("Title", null, Guid.Empty));
    }

    [Fact]
    public void Create_GeneratesUniqueIds()
    {
        var authorId = Guid.NewGuid();
        var b1 = Book.Create("Book 1", null, authorId);
        var b2 = Book.Create("Book 2", null, authorId);

        Assert.NotEqual(b1.Id, b2.Id);
    }
}
