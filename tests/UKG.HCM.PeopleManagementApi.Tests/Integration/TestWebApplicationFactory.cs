using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using UKG.HCM.PeopleManagementApi.Data;
using UKG.HCM.PeopleManagementApi.DTOs.AuthAPI;
using UKG.HCM.PeopleManagementApi.Services.Interfaces;
using Moq;

namespace UKG.HCM.PeopleManagementApi.Tests.Integration;

public class TestWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    private readonly string _dbName = $"PeopleManagementApiIntegrationTestDb_{Guid.NewGuid()}";
    private readonly Mock<IAuthService> _mockAuthService = new(MockBehavior.Strict);

    public TestWebApplicationFactory()
    {
        // Setup default behavior for auth service
        _mockAuthService.Setup(a => a.CreateUserAsync(It.IsAny<CreateUserDto>()))
            .ReturnsAsync(true);
        _mockAuthService.Setup(a => a.DeleteUserAsync(It.IsAny<string>()))
            .ReturnsAsync(true);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<PeopleContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add in-memory database
            services.AddDbContext<PeopleContext>(options =>
            {
                options.UseInMemoryDatabase(_dbName);
            });

            // Replace IAuthService with mock
            services.AddSingleton<IAuthService>(_mockAuthService.Object);

            // Add authentication test services
            services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();
            services.AddSingleton<IAuthorizationHandler, AllowAnonymous>();

            // Build the service provider
            var sp = services.BuildServiceProvider();

            // Create a scope to get scoped services
            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<PeopleContext>();
            var logger = scopedServices.GetRequiredService<ILogger<TestWebApplicationFactory<TProgram>>>();

            // Ensure database is created
            db.Database.EnsureCreated();

            try
            {
                // Seed the database with test data if needed
                // InitializeDbForTests(db);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred seeding the database. Error: {Message}", ex.Message);
            }
        });
    }

    // If needed, add methods to seed data or customize further
    public IAuthService GetAuthService() => _mockAuthService.Object;
}

// Fake policy evaluator for testing that bypasses authorization
public class FakePolicyEvaluator : IPolicyEvaluator
{
    public Task<AuthenticateResult> AuthenticateAsync(AuthorizationPolicy policy, HttpContext context)
    {
        // Always return success with admin claims
        var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, "Test User"),
            new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
            new Claim(ClaimTypes.Role, "Admin")
        }, "FakeScheme"));
        
        return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(principal, "FakeScheme")));
    }

    public Task<PolicyAuthorizationResult> AuthorizeAsync(AuthorizationPolicy policy, AuthenticateResult authenticationResult, HttpContext context, object? resource)
    {
        // Always allow access
        return Task.FromResult(PolicyAuthorizationResult.Success());
    }
}

// Fake authorization handler that allows anonymous access
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
