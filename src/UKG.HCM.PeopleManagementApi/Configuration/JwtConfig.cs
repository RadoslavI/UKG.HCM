namespace UKG.HCM.PeopleManagementApi.Configuration;

public sealed class JwtConfig
{
    public string Key { get; init; } = string.Empty;
    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
}