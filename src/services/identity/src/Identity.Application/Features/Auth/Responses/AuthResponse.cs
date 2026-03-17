namespace Identity.Application.Features.Auth.Responses;

public sealed record AuthResponse(
    string AccessToken,
    DateTime ExpiresAt,
    Guid UserId,
    string Name,
    string Email);
