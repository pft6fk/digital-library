using Engagement.Domain.Entities;

namespace Engagement.Domain.Tests;

public class RatingTests
{
    [Fact]
    public void Create_WithValidData_ReturnsRating()
    {
        var bookId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var rating = Rating.Create(bookId, userId, 4);

        Assert.NotEqual(Guid.Empty, rating.Id);
        Assert.Equal(bookId, rating.BookId);
        Assert.Equal(userId, rating.UserId);
        Assert.Equal(4, rating.Value);
        Assert.True(rating.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void Create_WithEmptyBookId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            Rating.Create(Guid.Empty, Guid.NewGuid(), 3));
    }

    [Fact]
    public void Create_WithEmptyUserId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            Rating.Create(Guid.NewGuid(), Guid.Empty, 3));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(6)]
    [InlineData(100)]
    public void Create_WithInvalidValue_ThrowsArgumentException(int value)
    {
        Assert.Throws<ArgumentException>(() =>
            Rating.Create(Guid.NewGuid(), Guid.NewGuid(), value));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void Create_WithValidValue_Succeeds(int value)
    {
        var rating = Rating.Create(Guid.NewGuid(), Guid.NewGuid(), value);
        Assert.Equal(value, rating.Value);
    }

    [Fact]
    public void Update_WithValidValue_ChangesValue()
    {
        var rating = Rating.Create(Guid.NewGuid(), Guid.NewGuid(), 3);
        rating.Update(5);

        Assert.Equal(5, rating.Value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void Update_WithInvalidValue_ThrowsArgumentException(int value)
    {
        var rating = Rating.Create(Guid.NewGuid(), Guid.NewGuid(), 3);

        Assert.Throws<ArgumentException>(() => rating.Update(value));
    }
}
