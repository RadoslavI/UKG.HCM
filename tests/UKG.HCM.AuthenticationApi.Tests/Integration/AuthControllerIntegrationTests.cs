using NUnit.Framework;
using System.Net.Http.Json;
using Moq;
using UKG.HCM.AuthenticationApi.Data.Entities;
using UKG.HCM.AuthenticationApi.DTOs.ChangePassword;
using UKG.HCM.AuthenticationApi.DTOs.CreateUser;
using UKG.HCM.AuthenticationApi.DTOs.DeleteUser;
using UKG.HCM.AuthenticationApi.DTOs.LoginUser;
using UKG.HCM.AuthenticationApi.Tests.Mocks;
using UKG.HCM.Shared.Constants;
using UKG.HCM.Shared.Utilities;

namespace UKG.HCM.AuthenticationApi.Tests.Integration
{
    [TestFixture]
    public class AuthControllerIntegrationTests
    {
        private TestWebApplicationFactory<Program> _factory = null!;
        private HttpClient _client = null!;

        [SetUp]
        public void SetUp()
        {
            _factory = new TestWebApplicationFactory<Program>();
            _client = _factory.CreateClient();
        }
        
        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        [Test]
        public async Task CreateUser_ReturnsSuccess()
        {
            var createUserDto = new IncomingCreateOrUpdateUserDto
            {
                Email = "newuser@example.com",
                FullName = "New User",
                Role = "User",
                Password = "Password123!"
            };

            var response = await _client.PostAsJsonAsync("/api/auth/register", createUserDto);
            response.EnsureSuccessStatusCode();

            var userServiceMock = _factory.GetUserServiceMock();
            userServiceMock.Verify(s => s.CreateUserAsync(It.IsAny<IncomingCreateOrUpdateUserDto>()), Times.Once);
        }

        [Test]
        public async Task Login_ReturnsToken()
        {
            var loginDto = new IncomingLoginUserDto("test@example.com", "Password123!");

            var response = await _client.PostAsJsonAsync("/api/auth/login", loginDto);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<OutgoingLoginUserDto>();

            Assert.That(result, Is.Not.Null);
            Assert.That("mock-jwt-token",Is.EqualTo(result!.Token));

            var userServiceMock = _factory.GetUserServiceMock();
            var tokenServiceMock = _factory.GetTokenServiceMock();

            userServiceMock.Verify(s => s.ValidateUserAsync(loginDto.Email, loginDto.Password), Times.Once);
            tokenServiceMock.Verify(t => t.GenerateToken(It.IsAny<User>()), Times.Once);
        }

        [Test]
        public async Task ChangePassword_ReturnsSuccess()
        {
            var db = MockDbContext.GetMockDbContext();

            var user = new User
            {
                FullName = "Test User",
                PasswordHash = PasswordHasher.HashPassword("OldPassword"),
                Email = "test email",
                Role = ApplicationRoles.Employee
            };
            
            await db.Users.AddAsync(user);

            var changePasswordDto = new IncomingChangePasswordDto(
                Email: user.Email,
                CurrentPassword: "OldPassword",
                NewPassword: "NewPass123!");

            var response = await _client.PostAsJsonAsync("/api/auth/change-password", changePasswordDto);
            response.EnsureSuccessStatusCode();

            var userServiceMock = _factory.GetUserServiceMock();
            userServiceMock.Verify(s => s.ChangePasswordAsync(
                changePasswordDto.Email, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword), Times.Once);
        }

        [Test]
        public async Task DeleteUser_ReturnsSuccess()
        {
            var deleteDto = new IncomingDeleteUserDTO("deleteuser@example.com");

            var request = new HttpRequestMessage(HttpMethod.Delete, "/api/auth/Delete");
            request.Content = JsonContent.Create(deleteDto);

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var userServiceMock = _factory.GetUserServiceMock();
            userServiceMock.Verify(s => s.DeleteUserAsync(deleteDto.Email), Times.Once);
        }
    }
}
