using ECommerceAppMorning.Server.Entities;

namespace ECommerceAppMorning.Server.Services
{
    public interface IJwtService
    {
        Task<string> GenerateAccessTokenAsync(ApplicationUser user);
        DateTime GetAccessTokenExpiration();
    }
}
