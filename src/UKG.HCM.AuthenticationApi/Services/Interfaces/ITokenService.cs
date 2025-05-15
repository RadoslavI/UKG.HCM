using UKG.HCM.AuthenticationApi.Data.Entities;

namespace UKG.HCM.AuthenticationApi.Services.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
}