using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using UKG.HCM.AuthenticationApi.Data;
using UKG.HCM.AuthenticationApi.Data.Entities;
using UKG.HCM.AuthenticationApi.DTOs.CreateUser;
using UKG.HCM.AuthenticationApi.Services;
using UKG.HCM.AuthenticationApi.Tests.Mocks;
using UKG.HCM.Shared.Constants;
using UKG.HCM.Shared.Utilities;

namespace UKG.HCM.AuthenticationApi.Tests.Services;

[TestFixture]
public class UserServiceTests
{
    private AuthContext _context;
    private UserService _service;
    private Mock<ILogger<UserService>> _mockLogger;

    [SetUp]
    public void Setup()
    {
        _context = MockDbContext.GetMockDbContext();
        _mockLogger = new Mock<ILogger<UserService>>();
        _service = new UserService(_context, _mockLogger.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    [Test]
    public async Task CreateUserAsync_ValidDto_ReturnsTrueAndSavesUser()
    {
        var dto = new IncomingCreateOrUpdateUserDto
        {
            Email = "john.doe@example.com",
            FullName = "John Doe",
            Role = ApplicationRoles.Employee,
            Password = PasswordHasher.GenerateRandomPassword()
        };

        var result = await _service.CreateUserAsync(dto);

        Assert.That(result.Success, Is.True);
        var savedUser = await _context.Users.FirstOrDefaultAsync(p => p.Email == dto.Email);
        Assert.That(savedUser, Is.Not.Null);
        Assert.That(savedUser.FullName, Is.EqualTo(dto.FullName));
        Assert.That(savedUser.Role, Is.EqualTo(dto.Role));
        Assert.That(savedUser.PasswordHash, Is.EqualTo(PasswordHasher.HashPassword(dto.Password)));
    }

    [Test]
    public async Task CreateUserAsync_ExistingEmail_ReturnsFalse()
    {
        var existingUser = new User
        {
            Id = Guid.NewGuid(),
            Email = "existing@example.com",
            FullName = "Existing",
            Role = ApplicationRoles.Employee,
            PasswordHash = PasswordHasher.HashPassword("secret")
        };
        await _context.Users.AddAsync(existingUser);
        await _context.SaveChangesAsync();

        var dto = new IncomingCreateOrUpdateUserDto
        {
            Email = "existing@example.com",
            FullName = "Duplicate",
            Role = ApplicationRoles.Manager
        };

        var result = await _service.CreateUserAsync(dto);

        Assert.That(result.Success, Is.False);
    }

    [Test]
    public async Task DeleteUserAsync_ExistingEmail_DeletesAndReturnsTrue()
    {
        var users = await MockDbContext.SeedUsers(_context);
        var existingUser = users.First();

        var result = await _service.DeleteUserAsync(existingUser.Email);

        Assert.That(result.Success, Is.True);
        var deletedUser = await _context.Users.FindAsync(existingUser.Id);
        Assert.That(deletedUser, Is.Null);
    }

    [Test]
    public async Task DeleteUserAsync_NonexistentEmail_ReturnsFalse()
    {
        await MockDbContext.SeedUsers(_context);

        var result = await _service.DeleteUserAsync("notfound@example.com");

        Assert.That(result.Success, Is.False);
    }

    [Test]
    public async Task ValidateUserAsync_CorrectCredentials_ReturnsUser()
    {
        var email = "user@example.com";
        var password = "secure123";
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            FullName = "Valid User",
            Role = ApplicationRoles.Employee,
            PasswordHash = PasswordHasher.HashPassword(password)
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var result = await _service.ValidateUserAsync(email, password);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Email, Is.EqualTo(email));
    }

    [Test]
    public async Task ValidateUserAsync_InvalidCredentials_ReturnsNull()
    {
        await MockDbContext.SeedUsers(_context);

        var result = await _service.ValidateUserAsync("invalid@example.com", "wrongpass");

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task ChangePasswordAsync_ValidCredentials_ChangesPasswordAndReturnsTrue()
    {
        var email = "changeme@example.com";
        var currentPassword = "oldpass";
        var newPassword = "newpass";

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            FullName = "Change Password",
            Role = ApplicationRoles.HRAdmin,
            PasswordHash = PasswordHasher.HashPassword(currentPassword)
        };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var result = await _service.ChangePasswordAsync(email, currentPassword, newPassword);

        Assert.That(result.Success, Is.True);

        var updatedUser = await _context.Users.FirstAsync(u => u.Email == email);
        Assert.That(updatedUser.PasswordHash, Is.EqualTo(PasswordHasher.HashPassword(newPassword)));
    }

    [Test]
    public async Task ChangePasswordAsync_InvalidCredentials_ReturnsFalse()
    {
        var email = "nonexistent@example.com";
        var result = await _service.ChangePasswordAsync(email, "wrongpass", "newpass");

        Assert.That(result.Success, Is.False);
    }
}
