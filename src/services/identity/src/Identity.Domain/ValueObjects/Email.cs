namespace Identity.Domain.ValueObjects;

public sealed class Email
{
    public string Value { get; }

    private Email(string value) => Value = value;

    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be empty.", nameof(value));

        var normalized = value.Trim().ToLowerInvariant();

        if (!normalized.Contains('@') || !normalized.Contains('.'))
            throw new ArgumentException("Email format is invalid.", nameof(value));

        return new Email(normalized);
    }

    public override string ToString() => Value;
    public override bool Equals(object? obj) => obj is Email other && Value == other.Value;
    public override int GetHashCode() => Value.GetHashCode();
}
