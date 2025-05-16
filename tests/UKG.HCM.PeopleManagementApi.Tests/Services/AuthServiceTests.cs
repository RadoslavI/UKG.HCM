using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using UKG.HCM.PeopleManagementApi.DTOs.AuthAPI;
using UKG.HCM.PeopleManagementApi.Services;

namespace UKG.HCM.PeopleManagementApi.Tests.Services;

[TestFixture]
public class AuthServiceTests
{
    private Mock<IConfiguration> _mockConfiguration;
    private Mock<ILogger<AuthService>> _mockLogger;
    private Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private HttpClient _httpClient;
    private AuthService _authService;
    private string _baseUrl = "http://authapi.example.com";

    [SetUp]
    public void Setup()
    {
        // Setup mocks
        _mockConfiguration = new Mock<IConfiguration>();
        _mockLogger = new Mock<ILogger<AuthService>>();
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        
        // Setup configuration to return base URL
        _mockConfiguration.Setup(c => c["AuthApiBaseUrl"]).Returns(_baseUrl);

        // Setup HTTP client with mock handler
        _httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri(_baseUrl)
        };

        // Create service under test with the mocked HttpClient
        _authService = new AuthService(_httpClient, _mockLogger.Object, _mockConfiguration.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _httpClient.Dispose();
    }

    [Test]
    public async Task CreateUserAsync_NullUserDto_ReturnsFalse()
    {
        // Act
        var result = await _authService.CreateUserAsync(null);
        
        // Assert
        Assert.That(result, Is.False);
        
        // Verify logger was called
        _mockLogger.Verify(
            x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Error), 
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("UserDto is null")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
        
        // Verify no HTTP request was made
        _mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Never(),
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>());
    }
    
    [Test]
    public async Task CreateUserAsync_Success_ReturnsTrue()
    {
        // Arrange
        var userDto = new CreateUserDto
        {
            Email = "test@example.com",
            FullName = "Test User",
            Role = "Employee"
        };

        // Setup HTTP handler mock to return success
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Created,
                Content = new StringContent("{\"success\":true}")
            });

        // Act
        var result = await _authService.CreateUserAsync(userDto);

        // Assert
        Assert.That(result, Is.True);
        
        // Verify HTTP request was made correctly - only check method
        _mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post),
            ItExpr.IsAny<CancellationToken>());
    }

    [Test]
    public async Task CreateUserAsync_ApiError_ReturnsFalse()
    {
        // Arrange
        var userDto = new CreateUserDto
        {
            Email = "test@example.com",
            FullName = "Test User",
            Role = "Employee"
        };

        // Setup HTTP handler mock to return error
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent("{\"error\":\"Invalid data\"}")
            });

        // Act
        var result = await _authService.CreateUserAsync(userDto);

        // Assert
        Assert.That(result, Is.False);
        
        // Verify error was logged
        _mockLogger.Verify(
            x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Error), 
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("API returned error")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }
    
    [Test]
    public async Task CreateUserAsync_SuccessStatusButSuccessFalse_ReturnsFalse()
    {
        // Arrange
        var userDto = new CreateUserDto
        {
            Email = "test@example.com",
            FullName = "Test User",
            Role = "Employee"
        };
    
        // Setup HTTP handler mock to return success status but success=false in body
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Created,
                Content = new StringContent("{\"success\":false}")
            });
    
        // Act
        var result = await _authService.CreateUserAsync(userDto);
    
        // Assert
        Assert.That(result, Is.False);
    }
    
    [Test]
    public async Task CreateUserAsync_MalformedJsonResponse_ReturnsFalse()
    {
        // Arrange
        var userDto = new CreateUserDto
        {
            Email = "test@example.com",
            FullName = "Test User",
            Role = "Employee"
        };
    
        // Setup HTTP handler mock to return malformed JSON
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Created,
                Content = new StringContent("{not valid json}")
            });
    
        // Act
        var result = await _authService.CreateUserAsync(userDto);
    
        // Assert
        Assert.That(result, Is.False);
        
        // Verify error was logged
        _mockLogger.Verify(
            x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Error), 
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error parsing")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Test]
    public async Task CreateUserAsync_Exception_ReturnsFalse()
    {
        // Arrange
        var userDto = new CreateUserDto
        {
            Email = "test@example.com",
            FullName = "Test User",
            Role = "Employee"
        };

        // Setup HTTP handler mock to throw exception
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Network error"));

        // Act
        var result = await _authService.CreateUserAsync(userDto);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task DeleteUserAsync_NullOrEmptyEmail_ReturnsFalse()
    {
        // Act & Assert - null email
        Assert.That(await _authService.DeleteUserAsync(null), Is.False);
        
        // Act & Assert - empty email
        Assert.That(await _authService.DeleteUserAsync(""), Is.False);
        
        // Act & Assert - whitespace email
        Assert.That(await _authService.DeleteUserAsync("   "), Is.False);
        
        // Verify logger was called
        _mockLogger.Verify(
            x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Error), 
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Email is null or empty")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.AtLeast(3));
        
        // Verify no HTTP request was made
        _mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Never(),
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>());
    }
    
    [Test]
    public async Task DeleteUserAsync_Success_ReturnsTrue()
    {
        // Arrange
        var email = "test@example.com";

        // Setup HTTP handler mock to return success
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

        // Act
        var result = await _authService.DeleteUserAsync(email);

        // Assert
        Assert.That(result, Is.True);
        
        // Only verify the HTTP method
        _mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Delete),
            ItExpr.IsAny<CancellationToken>());
    }

    [Test]
    public async Task DeleteUserAsync_ApiError_ReturnsFalse()
    {
        // Arrange
        var email = "test@example.com";

        // Setup HTTP handler mock to return error
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound,
                Content = new StringContent("{\"error\":\"User not found\"}")
            });

        // Act
        var result = await _authService.DeleteUserAsync(email);

        // Assert
        Assert.That(result, Is.False);
        
        // Verify error was logged
        _mockLogger.Verify(
            x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Error), 
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("API returned error")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }
    
    [Test]
    public async Task DeleteUserAsync_SuccessStatusButSuccessFalse_ReturnsFalse()
    {
        // Arrange
        var email = "test@example.com";
    
        // Setup HTTP handler mock to return success status but success=false in body
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"success\":false}")
            });
    
        // Act
        var result = await _authService.DeleteUserAsync(email);
    
        // Assert
        Assert.That(result, Is.False);
    }
    
    [Test]
    public async Task DeleteUserAsync_MalformedJsonResponse_ReturnsFalse()
    {
        // Arrange
        var email = "test@example.com";
    
        // Setup HTTP handler mock to return malformed JSON
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{not valid json}")
            });
    
        // Act
        var result = await _authService.DeleteUserAsync(email);
    
        // Assert
        Assert.That(result, Is.False);
        
        // Verify error was logged
        _mockLogger.Verify(
            x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Error), 
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error parsing")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Test]
    public async Task DeleteUserAsync_Exception_ReturnsFalse()
    {
        // Arrange
        var email = "test@example.com";

        // Setup HTTP handler mock to throw exception
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Network error"));

        // Act
        var result = await _authService.DeleteUserAsync(email);

        // Assert
        Assert.That(result, Is.False);
    }
}
