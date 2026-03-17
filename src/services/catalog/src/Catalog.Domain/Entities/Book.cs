namespace Catalog.Domain.Entities;

public sealed class Book
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = null!;
    public string? Description { get; private set; }
    public Guid AuthorId { get; private set; }

    private Book() { }

    public static Book Create(string title, string? description, Guid authorId)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Book title cannot be empty.", nameof(title));

        if (authorId == Guid.Empty)
            throw new ArgumentException("AuthorId is required.", nameof(authorId));

        return new Book
        {
            Id = Guid.NewGuid(),
            Title = title.Trim(),
            Description = description?.Trim(),
            AuthorId = authorId
        };
    }
}
