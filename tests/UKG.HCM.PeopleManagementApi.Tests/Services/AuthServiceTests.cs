using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using UKG.HCM.PeopleManagementApi.DTOs.AuthAPI;
using UKG.HCM.PeopleManagementApi.Services;

namespace UKG.HCM.PeopleManagementApi.Tests.Services
{
    public class AuthServiceTests
    {
        private Mock<ILogger<AuthService>> _loggerMock;
        private Mock<IConfiguration> _configurationMock;
        private const string RegisterEndpointKey = "AuthenticationApi:RegisterEndpoint";
        private const string DeleteEndpointKey = "AuthenticationApi:DeleteEndpoint";
        private const string RegisterUrl = "https://mocked.api.com/register";
        private const string DeleteUrl = "https://mocked.api.com/delete";

        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger<AuthService>>();
            _configurationMock = new Mock<IConfiguration>();
            _configurationMock.Setup(x => x[RegisterEndpointKey]).Returns(RegisterUrl);
            _configurationMock.Setup(x => x[DeleteEndpointKey]).Returns(DeleteUrl);
        }

        private HttpClient CreateHttpClient(HttpResponseMessage responseMessage, Action<Mock<HttpMessageHandler>> setupCallback = null)
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(responseMessage);

            setupCallback?.Invoke(handlerMock);
            return new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("https://mocked.api.com")
            };
        }

        [Test]
        public async Task CreateUserAsync_ShouldReturnTrue_WhenRequestIsSuccessful()
        {
            var httpClient = CreateHttpClient(new HttpResponseMessage(HttpStatusCode.OK));
            var service = new AuthService(httpClient, _loggerMock.Object, _configurationMock.Object);

            var result = await service.CreateUserAsync(new UserDto
            {
                Email = "test@example.com",
                FullName = "asdasd",
                Role = "Admin"
            });

            Assert.IsTrue(result);
        }

        [Test]
        public async Task CreateUserAsync_ShouldReturnFalse_WhenRequestFails()
        {
            var httpClient = CreateHttpClient(new HttpResponseMessage(HttpStatusCode.BadRequest));
            var service = new AuthService(httpClient, _loggerMock.Object, _configurationMock.Object);

            var result = await service.CreateUserAsync(new UserDto
            {
                Email = "test@example.com",
                FullName = "asdasd",
                Role = "Admin"
            });

            Assert.IsFalse(result);
        }

        [Test]
        public async Task CreateUserAsync_ShouldReturnFalse_OnException()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Network error"));

            var httpClient = new HttpClient(handlerMock.Object);
            var service = new AuthService(httpClient, _loggerMock.Object, _configurationMock.Object);

            var result = await service.CreateUserAsync(new UserDto
            {
                Email = "test@example.com", 
                FullName= "asdasd",
                Role = "Admin"
            });

            Assert.IsFalse(result);
        }

        [Test]
        public async Task DeleteUserAsync_ShouldReturnTrue_WhenRequestIsSuccessful()
        {
            var httpClient = CreateHttpClient(new HttpResponseMessage(HttpStatusCode.OK));
            var service = new AuthService(httpClient, _loggerMock.Object, _configurationMock.Object);

            var result = await service.DeleteUserAsync("test@example.com");

            Assert.IsTrue(result);
        }

        [Test]
        public async Task DeleteUserAsync_ShouldReturnFalse_WhenRequestFails()
        {
            var httpClient = CreateHttpClient(new HttpResponseMessage(HttpStatusCode.NotFound));
            var service = new AuthService(httpClient, _loggerMock.Object, _configurationMock.Object);

            var result = await service.DeleteUserAsync("test@example.com");

            Assert.IsFalse(result);
        }

        [Test]
        public async Task DeleteUserAsync_ShouldReturnFalse_OnException()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new Exception("Unexpected error"));

            var httpClient = new HttpClient(handlerMock.Object);
            var service = new AuthService(httpClient, _loggerMock.Object, _configurationMock.Object);

            var result = await service.DeleteUserAsync("test@example.com");

            Assert.IsFalse(result);
        }
    }
}
