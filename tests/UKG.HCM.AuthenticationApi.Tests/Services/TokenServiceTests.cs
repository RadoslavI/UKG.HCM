using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using UKG.HCM.AuthenticationApi.Data.Entities;
using UKG.HCM.AuthenticationApi.Services;

namespace UKG.HCM.AuthenticationApi.Tests.Services;

[TestFixture]
public class TokenServiceTests
{
    private TokenService _service;

    [SetUp]
    public void Setup()
    {
        var inMemorySettings = new Dictionary<string, string>
        {
            { "Jwt:Key", "supersecretkeythatshouldbeatleast32chars" },
            { "Jwt:Issuer", "AuthApi" },
            { "Jwt:Audience", "PeopleApi" },
            { "Jwt:ExpiryInMinutes", "60" }
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        _service = new TokenService(configuration);
    }

    [Test]
    public void GenerateToken_ValidUser_ReturnsValidJwtToken()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = "John Doe",
            Email = "john.doe@example.com",
            Role = "Employee"
        };

        // Act
        var token = _service.GenerateToken(user);

        // Assert
        Assert.That(token, Is.Not.Null.And.Not.Empty);

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        Assert.That(jwt.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value, Is.EqualTo(user.Id.ToString()));
        Assert.That(jwt.Claims.First(c => c.Type == ClaimTypes.Name).Value, Is.EqualTo(user.FullName));
        Assert.That(jwt.Claims.First(c => c.Type == ClaimTypes.Email).Value, Is.EqualTo(user.Email));
        Assert.That(jwt.Claims.First(c => c.Type == ClaimTypes.Role).Value, Is.EqualTo(user.Role));
        Assert.That(jwt.Issuer, Is.EqualTo("AuthApi"));
        Assert.That(jwt.Audiences.Contains("PeopleApi"), Is.True);
        Assert.That(jwt.ValidTo > DateTime.UtcNow, Is.True);
    }
}
