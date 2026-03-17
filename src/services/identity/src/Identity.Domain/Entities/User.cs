using Identity.Domain.ValueObjects;

namespace Identity.Domain.Entities;

public sealed class User
{
    public Guid Id { get; private set; }
    public Email Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public string Name { get; private set; } = null!;

    private User() { }

    public static User Create(string email, string passwordHash, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));

        return new User
        {
            Id = Guid.NewGuid(),
            Email = Email.Create(email),
            PasswordHash = passwordHash,
            Name = name.Trim()
        };
    }
}
