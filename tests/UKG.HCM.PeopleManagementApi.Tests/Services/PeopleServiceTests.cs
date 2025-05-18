using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using UKG.HCM.PeopleManagementApi.Data;
using UKG.HCM.PeopleManagementApi.Data.Entities;
using UKG.HCM.PeopleManagementApi.DTOs.AuthAPI;
using UKG.HCM.PeopleManagementApi.DTOs.Person.Create;
using UKG.HCM.PeopleManagementApi.DTOs.Person.Update;
using UKG.HCM.PeopleManagementApi.Services;
using UKG.HCM.PeopleManagementApi.Services.Interfaces;
using UKG.HCM.PeopleManagementApi.Tests.Mocks;
using UKG.HCM.Shared.Constants;

namespace UKG.HCM.PeopleManagementApi.Tests.Services;

[TestFixture]
public class PeopleServiceTests
{
    private PeopleContext _context;
    private PeopleService _service;
    private Mock<IAuthService>  _mockAuthService;
    private Mock<ILogger<PeopleService>>  _mockLogger;

    [SetUp]
    public void Setup()
    {
        _context = MockDbContext.GetMockDbContext();
        _mockAuthService = new Mock<IAuthService>();
        _mockLogger = new Mock<ILogger<PeopleService>>();
        
        // Set default behavior for auth service to prevent exceptions
        _mockAuthService.Setup(a => a.CreateUserAsync(It.IsAny<UserDto>()))
            .ReturnsAsync(true);
        _mockAuthService.Setup(a => a.DeleteUserAsync(It.IsAny<string>()))
            .ReturnsAsync(true);
            
        _service = new PeopleService(_context, _mockAuthService.Object, _mockLogger.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }
    
    [Test]
    public async Task GetPeopleAsync_ReturnsAllPeople()
    {
        // Arrange
        await MockDbContext.SeedPeople(_context);
        
        // Act
        var result = await _service.GetPeopleAsync();
        
        // Assert
        Assert.That(result.Count(), Is.EqualTo(3));
    }
    
    [Test]
    public async Task GetPersonByIdAsync_ExistingId_ReturnsPerson()
    {
        // Arrange
        var people = await MockDbContext.SeedPeople(_context);
        var existingPerson = people.First();
        
        // Act
        var result = await _service.GetPersonByIdAsync(existingPerson.Id);
        
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(existingPerson.Id));
        Assert.That(result.FirstName, Is.EqualTo(existingPerson.FirstName));
        Assert.That(result.LastName, Is.EqualTo(existingPerson.LastName));
    }
    
    [Test]
    public async Task GetPersonByIdAsync_NonexistentId_ReturnsNull()
    {
        // Arrange
        await MockDbContext.SeedPeople(_context);
        
        // Act
        var result = await _service.GetPersonByIdAsync(Guid.NewGuid());
        
        // Assert
        Assert.That(result, Is.Null);
    }
    
    [Test]
    public async Task CreatePersonAsync_ValidDto_ReturnsNewId()
    {
        // Arrange
        var dto = new IncomingCreatePersonDTO
        (
            FirstName: "John",
            LastName: "Doe",
            Email: "john.doe@example.com",
            Role: "Employee",
            HireDate: DateTime.Now
        );
        
        // Setup auth service to return success for this create operation
        _mockAuthService
            .Setup(a => a.CreateUserAsync(It.IsAny<UserDto>()))
            .ReturnsAsync(true);
        
        // Act
        var result = await _service.CreatePersonAsync(dto);
        
        // Assert
        Assert.That(result, Is.Not.EqualTo(Guid.Empty));
        
        var savedPerson = await _context.People.FirstOrDefaultAsync(p => p.Id == result);
        Assert.That(savedPerson, Is.Not.Null);
        Assert.That(savedPerson.FirstName, Is.EqualTo(dto.FirstName));
        Assert.That(savedPerson.LastName, Is.EqualTo(dto.LastName));
        Assert.That(savedPerson.Email, Is.EqualTo(dto.Email));
    }
    
    [Test]
    public async Task UpdatePersonAsync_ExistingId_UpdatesAndReturnsTrue()
    {
        // Arrange
        var people = await MockDbContext.SeedPeople(_context);
        var existingPerson = people.First();

        var updateDto = new IncomingUpdatePersonDTO(
            "John",
            "Doe",
            "john.doe@ukg.com",
            ApplicationRoles.Employee
        );
        
        // Act
        var result = await _service.UpdatePersonAsync(existingPerson.Id, updateDto);
        
        // Assert
        Assert.That(result, Is.True);
        
        var updatedPerson = await _context.People.FindAsync(existingPerson.Id);
        Assert.That(updatedPerson, Is.Not.Null);
        Assert.That(updatedPerson.FirstName, Is.EqualTo(updateDto.FirstName));
        Assert.That(updatedPerson.LastName, Is.EqualTo(updateDto.LastName));
        Assert.That(updatedPerson.Email, Is.EqualTo(updateDto.Email));
        Assert.That(updatedPerson.Role.ToString(), Is.EqualTo(updateDto.Role));
    }
    
    [Test]
    public async Task UpdatePersonAsync_NonexistentId_ReturnsFalse()
    {
        // Arrange
        await MockDbContext.SeedPeople(_context);
        
        var updateDto = new IncomingUpdatePersonDTO(
            "John",
            "Doe",
            "john.doe@ukg.com",
            ApplicationRoles.Employee
        );
        
        // Act
        var result = await _service.UpdatePersonAsync(Guid.NewGuid(), updateDto);
        
        // Assert
        Assert.That(result, Is.False);
    }
    
    [Test]
    public async Task DeletePersonAsync_ExistingId_DeletesAndReturnsTrue()
    {
        // Arrange
        var people = await MockDbContext.SeedPeople(_context);
        var existingPerson = people.First();
        
        // Setup auth service to return success for the delete operation
        _mockAuthService.Setup(a => a.DeleteUserAsync(existingPerson.Email))
            .ReturnsAsync(true);
        
        // Act
        var result = await _service.DeletePersonAsync(existingPerson.Id);
        
        // Assert
        Assert.That(result, Is.True);
        
        var deletedPerson = await _context.People.FindAsync(existingPerson.Id);
        Assert.That(deletedPerson, Is.Null);
        
        // Verify auth service was called
        _mockAuthService.Verify(a => a.DeleteUserAsync(existingPerson.Email), Times.Once);
    }
    
    [Test]
    public async Task DeletePersonAsync_NonexistentId_ReturnsFalse()
    {
        // Arrange
        await MockDbContext.SeedPeople(_context);
        
        // Act
        var result = await _service.DeletePersonAsync(Guid.NewGuid());
        
        // Assert
        Assert.That(result, Is.False);
    }
}
