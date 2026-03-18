namespace Engagement.Domain.Entities;

public sealed class Rating
{
    public Guid Id { get; private set; }
    public Guid BookId { get; private set; }
    public Guid UserId { get; private set; }
    public int Value { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Rating() { }

    public static Rating Create(Guid bookId, Guid userId, int value)
    {
        if (bookId == Guid.Empty)
            throw new ArgumentException("BookId is required.", nameof(bookId));
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId is required.", nameof(userId));
        if (value < 1 || value > 5)
            throw new ArgumentException("Rating value must be between 1 and 5.", nameof(value));

        return new Rating
        {
            Id = Guid.NewGuid(),
            BookId = bookId,
            UserId = userId,
            Value = value,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(int newValue)
    {
        if (newValue < 1 || newValue > 5)
            throw new ArgumentException("Rating value must be between 1 and 5.", nameof(newValue));

        Value = newValue;
        CreatedAt = DateTime.UtcNow;
    }
}
