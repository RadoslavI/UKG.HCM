using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using UKG.HCM.AuthenticationApi.Data;
using UKG.HCM.AuthenticationApi.Data.Entities;
using UKG.HCM.AuthenticationApi.DTOs.CreateUser;
using UKG.HCM.AuthenticationApi.Services.Interfaces;
using UKG.HCM.AuthenticationApi.Tests.Mocks;
using UKG.HCM.Shared.Constants;
using UKG.HCM.Shared.Utilities;

namespace UKG.HCM.AuthenticationApi.Tests.Integration
{
    public class TestWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        private readonly string _dbName = $"AuthApiIntegrationTestDb_{Guid.NewGuid()}";
        private readonly Mock<IUserService> _mockUserService = new(MockBehavior.Strict);
        private readonly Mock<ITokenService> _mockTokenService = new(MockBehavior.Strict);

        public TestWebApplicationFactory()
        {
            // Setup default behavior for IUserService
            _mockUserService.Setup(s => s.CreateUserAsync(It.IsAny<IncomingCreateUserDto>()))
                .ReturnsAsync(true);

            _mockUserService.Setup(s => s.ValidateUserAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((string email, string password) => new User
                {
                    Id = Guid.NewGuid(),
                    Email = email,
                    FullName = "Test User",
                    Role = ApplicationRoles.HRAdmin,
                    PasswordHash = PasswordHasher.HashPassword(password)
                });

            _mockUserService.Setup(s => s.ChangePasswordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            _mockUserService.Setup(s => s.DeleteUserAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            // Setup default behavior for ITokenService
            _mockTokenService.Setup(s => s.GenerateToken(It.IsAny<User>()))
                .Returns("mock-jwt-token");
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove existing AuthContext registration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AuthContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add in-memory EF Core database
                services.AddDbContext<AuthContext>(options =>
                {
                    options.UseInMemoryDatabase(_dbName);
                });

                // Replace IUserService and ITokenService with mocks
                services.AddSingleton<IUserService>(_mockUserService.Object);
                services.AddSingleton<ITokenService>(_mockTokenService.Object);

                // Add fake authentication/authorization handlers for tests
                services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();
                services.AddSingleton<IAuthorizationHandler, AllowAnonymous>();

                // Build service provider and ensure DB creation
                var sp = services.BuildServiceProvider();

                using var scope = sp.CreateScope();
                var scopedServices = scope.ServiceProvider;

                var db = scopedServices.GetRequiredService<AuthContext>();
                var logger = scopedServices.GetRequiredService<ILogger<TestWebApplicationFactory<TProgram>>>();

                db.Database.EnsureCreated();

                try
                {
                    _ = MockDbContext.SeedUsers(db);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error occurred seeding the database. Message: {Message}", ex.Message);
                }
            });
        }

        // Expose mocks for customization in tests
        public Mock<IUserService> GetUserServiceMock() => _mockUserService;
        public Mock<ITokenService> GetTokenServiceMock() => _mockTokenService;
    }

    // Fake policy evaluator to bypass authentication in integration tests
    public class FakePolicyEvaluator : IPolicyEvaluator
    {
        public Task<AuthenticateResult> AuthenticateAsync(AuthorizationPolicy policy, HttpContext context)
        {
            var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "Test User"),
                new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
                new Claim(ClaimTypes.Email, "test email"),
                new Claim(ClaimTypes.Role, ApplicationRoles.HRAdmin)
            }, "FakeScheme"));

            return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(principal, "FakeScheme")));
        }

        public Task<PolicyAuthorizationResult> AuthorizeAsync(AuthorizationPolicy policy, AuthenticateResult authenticationResult,
            HttpContext context, object? resource)
        {
            return Task.FromResult(PolicyAuthorizationResult.Success());
        }
    }

    // Fake authorization handler that always approves authorization requirements
    public class AllowAnonymous : IAuthorizationHandler
    {
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            foreach (var requirement in context.PendingRequirements.ToList())
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
