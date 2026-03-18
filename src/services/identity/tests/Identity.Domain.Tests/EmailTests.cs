using Identity.Domain.ValueObjects;

namespace Identity.Domain.Tests;

public class EmailTests
{
    [Fact]
    public void Create_WithValidEmail_ReturnsEmail()
    {
        var email = Email.Create("test@example.com");

        Assert.Equal("test@example.com", email.Value);
    }

    [Fact]
    public void Create_NormalizesToLowercase()
    {
        var email = Email.Create("TEST@EXAMPLE.COM");

        Assert.Equal("test@example.com", email.Value);
    }

    [Fact]
    public void Create_TrimsWhitespace()
    {
        var email = Email.Create("  test@example.com  ");

        Assert.Equal("test@example.com", email.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithEmptyValue_ThrowsArgumentException(string? value)
    {
        Assert.Throws<ArgumentException>(() => Email.Create(value!));
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("missing-at.com")]
    [InlineData("missing@dot")]
    public void Create_WithInvalidFormat_ThrowsArgumentException(string value)
    {
        Assert.Throws<ArgumentException>(() => Email.Create(value));
    }

    [Fact]
    public void Equals_SameValue_ReturnsTrue()
    {
        var email1 = Email.Create("test@example.com");
        var email2 = Email.Create("test@example.com");

        Assert.Equal(email1, email2);
    }

    [Fact]
    public void Equals_DifferentCase_ReturnsTrue()
    {
        var email1 = Email.Create("Test@Example.com");
        var email2 = Email.Create("test@example.com");

        Assert.Equal(email1, email2);
    }

    [Fact]
    public void Equals_DifferentValue_ReturnsFalse()
    {
        var email1 = Email.Create("a@example.com");
        var email2 = Email.Create("b@example.com");

        Assert.NotEqual(email1, email2);
    }

    [Fact]
    public void ToString_ReturnsValue()
    {
        var email = Email.Create("test@example.com");

        Assert.Equal("test@example.com", email.ToString());
    }

    [Fact]
    public void GetHashCode_SameEmail_SameHash()
    {
        var email1 = Email.Create("test@example.com");
        var email2 = Email.Create("TEST@example.com");

        Assert.Equal(email1.GetHashCode(), email2.GetHashCode());
    }
}
