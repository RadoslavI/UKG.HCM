using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using UKG.HCM.PeopleManagementApi.DTOs.AuthAPI;
using UKG.HCM.PeopleManagementApi.Services;
using UKG.HCM.PeopleManagementApi.Services.Interfaces;

namespace UKG.HCM.PeopleManagementApi.Tests.Integration;

[TestFixture]
public class AuthServiceIntegrationTests
{
    private TestWebApplicationFactory<Program> _factory;
    private IAuthService _authService;
    private Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private HttpClient _httpClient;
    private IConfiguration _configuration;
    private ILogger<AuthService> _logger;

    [SetUp]
    public void Setup()
    {
        // Create mock HTTP handler and client
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("http://authapi.example.com")
        };
        
        // Create factory to get services
        _factory = new TestWebApplicationFactory<Program>();
        
        // Get configuration and logger from DI container
        _configuration = _factory.Services.GetRequiredService<IConfiguration>();
        _logger = _factory.Services.GetRequiredService<ILogger<AuthService>>();
        
        // Setup mock handler responses
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"success\":true}")
            });
        
        // Create AuthService with mocked HttpClient
        _authService = new AuthService(_httpClient, _logger, _configuration);
    }

    [TearDown]
    public void TearDown()
    {
        _httpClient.Dispose();
        _factory.Dispose();
    }

    [Test]
    public async Task CreateUserAsync_Integration_ReturnsExpectedResult()
    {
        // Arrange
        var userDto = new CreateUserDto
        {
            Email = "integration.test@example.com",
            FullName = "Integration Test User",
            Role = "Employee"
        };

        // Act
        var result = await _authService.CreateUserAsync(userDto);

        // Assert
        Assert.That(result, Is.True);
        
        // Verify HTTP request was made correctly
        _mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.AtLeastOnce(),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post),
            ItExpr.IsAny<CancellationToken>());
    }

    [Test]
    public async Task DeleteUserAsync_Integration_ReturnsExpectedResult()
    {
        // Arrange
        var email = "integration.test@example.com";

        // Act
        var result = await _authService.DeleteUserAsync(email);

        // Assert
        Assert.That(result, Is.True);
        
        // Verify HTTP request was made correctly
        _mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.AtLeastOnce(),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Delete),
            ItExpr.IsAny<CancellationToken>());
    }
}
