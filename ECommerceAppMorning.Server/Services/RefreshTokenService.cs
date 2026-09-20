using System.Security.Cryptography;

using ECommerceAppMorning.Server.Entities;
using ECommerceAppMorning.Server.Models;

using Microsoft.Extensions.Options;

namespace ECommerceAppMorning.Server.Services;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly JwtSettings _settings;

    public RefreshTokenService(
        IOptions<JwtSettings> settings)
    {
        _settings = settings.Value;
    }

    public RefreshToken Generate(int userId)
    {
        return new RefreshToken
        {
            Token = Convert.ToBase64String(
                RandomNumberGenerator.GetBytes(64)),

            UserId = userId,

            ExpiresAt = DateTime.UtcNow.AddDays(
                _settings.RefreshTokenExpirationDays),

            CreatedAt = DateTime.UtcNow
        };
    }
}