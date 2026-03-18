using Catalog.Domain.Entities;

namespace Catalog.Domain.Tests;

public class AuthorTests
{
    [Fact]
    public void Create_WithValidData_ReturnsAuthor()
    {
        var birthDate = new DateTime(1990, 1, 15);
        var author = Author.Create("Leo Tolstoy", "Russian novelist", birthDate);

        Assert.NotEqual(Guid.Empty, author.Id);
        Assert.Equal("Leo Tolstoy", author.FullName);
        Assert.Equal("Russian novelist", author.Bio);
        Assert.Equal(birthDate, author.BirthDate);
    }

    [Fact]
    public void Create_WithNullOptionalFields_ReturnsAuthor()
    {
        var author = Author.Create("Author Name", null, null);

        Assert.Equal("Author Name", author.FullName);
        Assert.Null(author.Bio);
        Assert.Null(author.BirthDate);
    }

    [Fact]
    public void Create_TrimsFullName()
    {
        var author = Author.Create("  Leo Tolstoy  ", null, null);

        Assert.Equal("Leo Tolstoy", author.FullName);
    }

    [Fact]
    public void Create_TrimsBio()
    {
        var author = Author.Create("Name", "  Some bio  ", null);

        Assert.Equal("Some bio", author.Bio);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithEmptyName_ThrowsArgumentException(string? name)
    {
        Assert.Throws<ArgumentException>(() => Author.Create(name!, null, null));
    }

    [Fact]
    public void Create_GeneratesUniqueIds()
    {
        var a1 = Author.Create("Author 1", null, null);
        var a2 = Author.Create("Author 2", null, null);

        Assert.NotEqual(a1.Id, a2.Id);
    }
}
