using Identity.Domain.Entities;

namespace Identity.Application.Services;

public interface IJwtTokenService
{
    string GenerateToken(User user);
}
