using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Sencecon.Application.Common.Interfaces;

namespace Sencecon.Infrastructure.Identity;

public class CurrentUserService : ICurrentUserService
{
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        var subClaim = httpContextAccessor.HttpContext?.User
            .FindFirstValue(JwtRegisteredClaimNames.Sub);

        UserId = subClaim is not null && Guid.TryParse(subClaim, out var userId)
            ? userId
            : null;
    }

    public Guid? UserId { get; }
}
