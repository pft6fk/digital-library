namespace Engagement.Domain.Entities;

public sealed class Comment
{
    public Guid Id { get; private set; }
    public Guid BookId { get; private set; }
    public Guid UserId { get; private set; }
    public string UserName { get; private set; } = null!;
    public string Text { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }

    private Comment() { }

    public static Comment Create(Guid bookId, Guid userId, string userName, string text)
    {
        if (bookId == Guid.Empty)
            throw new ArgumentException("BookId is required.", nameof(bookId));
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId is required.", nameof(userId));
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Comment text cannot be empty.", nameof(text));

        return new Comment
        {
            Id = Guid.NewGuid(),
            BookId = bookId,
            UserId = userId,
            UserName = userName,
            Text = text.Trim(),
            CreatedAt = DateTime.UtcNow
        };
    }
}
