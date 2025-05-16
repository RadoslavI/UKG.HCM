using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using UKG.HCM.AuthenticationApi.Data;
using UKG.HCM.AuthenticationApi.DTOs.CreateUser;
using UKG.HCM.AuthenticationApi.Services;
using UKG.HCM.AuthenticationApi.Tests.Mocks;
using UKG.HCM.Shared.Constants;

namespace UKG.HCM.AuthenticationApi.Tests.Services;

[TestFixture]
public class UserServiceTests
{
    private AuthContext _context;
    private UserService _service;
    private Mock<ILogger<UserService>>  _mockLogger;

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
    public async Task CreateUserAsync_ValidDto_ReturnsNewId()
    {
        // Arrange
        var dto = new IncomingCreateUserDto
        {
            Email = "john.doe@example.com",
            FullName = "John Doe",
            Role = ApplicationRoles.Employee,
            Password = "testpasword123"
        };
        
        // Act
        var result = await _service.CreateUserAsync(dto);
        
        // Assert
        Assert.That(result, Is.True);
        var savedUser = await _context.Users.FirstOrDefaultAsync(p => p.Email == dto.Email);
        Assert.That(savedUser, Is.Not.Null);
        Assert.That(savedUser.FullName, Is.EqualTo(dto.FullName));
        Assert.That(savedUser.Role, Is.EqualTo(dto.Role));
    }
    
    [Test]
    public async Task DeleteUserAsync_ExistingId_DeletesAndReturnsTrue()
    {
        // Arrange
        var users = await MockDbContext.SeedUsers(_context);
        var existingUser = users.First();
        
        // Act
        var result = await _service.DeleteUserAsync(existingUser.Email);
        
        // Assert
        Assert.That(result, Is.True);
        
        var deletedPerson = await _context.Users.FindAsync(existingUser.Id);
        Assert.That(deletedPerson, Is.Null);
    }
    
    [Test]
    public async Task DeletePersonAsync_NonexistentId_ReturnsFalse()
    {
        // Arrange
        await MockDbContext.SeedUsers(_context);
        
        // Act
        var result = await _service.DeleteUserAsync("random email");
        
        // Assert
        Assert.That(result, Is.False);
    }
}
