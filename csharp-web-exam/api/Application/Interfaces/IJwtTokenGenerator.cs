using api.Domain.Entities;

namespace api.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
