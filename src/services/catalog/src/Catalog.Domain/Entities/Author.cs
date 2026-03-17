namespace Catalog.Domain.Entities;

public sealed class Author
{
    public Guid Id { get; private set; }
    public string FullName { get; private set; } = null!;
    public string? Bio { get; private set; }
    public DateTime? BirthDate { get; private set; }

    private Author() { }

    public static Author Create(string fullName, string? bio, DateTime? birthDate)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Author name cannot be empty.", nameof(fullName));

        return new Author
        {
            Id = Guid.NewGuid(),
            FullName = fullName.Trim(),
            Bio = bio?.Trim(),
            BirthDate = birthDate
        };
    }
}
