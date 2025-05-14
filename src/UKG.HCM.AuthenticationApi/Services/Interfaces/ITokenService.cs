using UKG.HCM.AuthenticationApi.Models;

namespace UKG.HCM.AuthenticationApi.Services.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
}