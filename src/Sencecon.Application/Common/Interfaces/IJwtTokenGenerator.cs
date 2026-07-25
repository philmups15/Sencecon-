using Sencecon.Domain.Entities;

namespace Sencecon.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
