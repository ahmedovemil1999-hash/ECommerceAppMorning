using ECommerceAppMorning.Server.Entities;

namespace ECommerceAppMorning.Server.Services
{
    public interface IRefreshTokenService
    {
        RefreshToken Generate(int userId);
    }
}
