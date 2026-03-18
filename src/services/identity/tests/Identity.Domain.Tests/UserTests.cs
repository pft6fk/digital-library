using Identity.Domain.Entities;

namespace Identity.Domain.Tests;

public class UserTests
{
    [Fact]
    public void Create_WithValidData_ReturnsUser()
    {
        var user = User.Create("test@example.com", "hashedPassword", "John Doe");

        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.Equal("test@example.com", user.Email.Value);
        Assert.Equal("hashedPassword", user.PasswordHash);
        Assert.Equal("John Doe", user.Name);
    }

    [Fact]
    public void Create_NormalizesEmail()
    {
        var user = User.Create("  TEST@Example.COM  ", "hash", "Name");

        Assert.Equal("test@example.com", user.Email.Value);
    }

    [Fact]
    public void Create_TrimsName()
    {
        var user = User.Create("test@example.com", "hash", "  John Doe  ");

        Assert.Equal("John Doe", user.Name);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithEmptyName_ThrowsArgumentException(string? name)
    {
        Assert.Throws<ArgumentException>(() =>
            User.Create("test@example.com", "hash", name!));
    }

    [Fact]
    public void Create_GeneratesUniqueIds()
    {
        var user1 = User.Create("a@example.com", "hash", "User1");
        var user2 = User.Create("b@example.com", "hash", "User2");

        Assert.NotEqual(user1.Id, user2.Id);
    }
}
