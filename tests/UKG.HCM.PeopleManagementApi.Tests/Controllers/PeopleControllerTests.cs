using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using UKG.HCM.PeopleManagementApi.Controllers;
using UKG.HCM.PeopleManagementApi.DTOs.Person.Create;
using UKG.HCM.PeopleManagementApi.DTOs.Person.Get;
using UKG.HCM.PeopleManagementApi.DTOs.Person.Update;
using UKG.HCM.PeopleManagementApi.Services.Interfaces;
using UKG.HCM.Shared.Constants;
using UKG.HCM.Shared.Utilities;

namespace UKG.HCM.PeopleManagementApi.Tests.Controllers;

[TestFixture]
public class PeopleControllerTests
{
    private Mock<IPeopleService> _mockPeopleService;
    private PeopleController _controller;

    [SetUp]
    public void Setup()
    {
        _mockPeopleService = new Mock<IPeopleService>(MockBehavior.Strict);
        _controller = new PeopleController(_mockPeopleService.Object);
    }

    [Test]
    public async Task GetPeople_ReturnsOkResultWithPeople()
    {
        // Arrange
        var person1Id = Guid.NewGuid();
        var person2Id = Guid.NewGuid();
        var expectedPeople = new List<OutgoingGetPersonDTO>
        {
            new(person1Id, "John", "Doe", "john.doe@example.com", ApplicationRoles.Employee, DateTime.Now),
            new(person2Id, "Jane", "Smith", "jane.smith@example.com", ApplicationRoles.Manager, DateTime.Now)
        };
        _mockPeopleService.Setup(s => s.GetPeopleAsync()).ReturnsAsync(expectedPeople);

        // Act
        var result = await _controller.GetPeople();

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = (OkObjectResult)result.Result;
        Assert.That(okResult.Value, Is.InstanceOf<IEnumerable<OutgoingGetPersonDTO>>());
        var returnedPeople = (IEnumerable<OutgoingGetPersonDTO>)okResult.Value;
        Assert.That(returnedPeople.Count(), Is.EqualTo(expectedPeople.Count));
        
        // Verify the service was called once
        _mockPeopleService.Verify(s => s.GetPeopleAsync(), Times.Once);
    }

    [Test]
    public async Task GetPerson_WithExistingId_ReturnsOkResultWithPerson()
    {
        // Arrange
        var personId = Guid.NewGuid();
        var expectedPerson = new OutgoingGetPersonDTO(
            personId, 
            "John", 
            "Doe", 
            "john.doe@example.com", 
            ApplicationRoles.Employee,
            DateTime.Now
        );
        _mockPeopleService.Setup(s => s.GetPersonByIdAsync(personId)).ReturnsAsync(expectedPerson);

        // Act
        var result = await _controller.GetPerson(personId);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = (OkObjectResult)result.Result;
        Assert.That(okResult.Value, Is.InstanceOf<OutgoingGetPersonDTO>());
        var returnedPerson = (OutgoingGetPersonDTO)okResult.Value;
        Assert.That(returnedPerson.Id, Is.EqualTo(personId));
        
        // Verify the service was called once with the correct ID
        _mockPeopleService.Verify(s => s.GetPersonByIdAsync(personId), Times.Once);
    }

    [Test]
    public async Task GetPerson_WithNonexistentId_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        _mockPeopleService.Setup(s => s.GetPersonByIdAsync(nonExistentId)).ReturnsAsync((OutgoingGetPersonDTO)null);

        // Act
        var result = await _controller.GetPerson(nonExistentId);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
        
        // Verify the service was called once with the correct ID
        _mockPeopleService.Verify(s => s.GetPersonByIdAsync(nonExistentId), Times.Once);
    }

    [Test]
    public async Task CreatePerson_ReturnsCreatedAtActionResultWithId()
    {
        // Arrange
        var personDto = new IncomingCreatePersonDTO(
            FirstName: "John",
            LastName: "Doe",
            Email: "john.doe@example.com",
            Role: ApplicationRoles.Employee,
            HireDate: DateTime.Now
        );
        var expectedId = Guid.NewGuid();
        
        // Use It.IsAny for object comparison to avoid reference equality issues
        _mockPeopleService
            .Setup(s => s.CreatePersonAsync(It.IsAny<IncomingCreatePersonDTO>()))
            .ReturnsAsync(expectedId);

        // Act
        var result = await _controller.CreatePerson(personDto);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<CreatedAtActionResult>());
        var createdAtActionResult = (CreatedAtActionResult)result.Result;
        Assert.That(createdAtActionResult.ActionName, Is.EqualTo(nameof(PeopleController.GetPerson)));
        Assert.That(createdAtActionResult.RouteValues["id"], Is.EqualTo(expectedId));
        Assert.That(createdAtActionResult.Value, Is.EqualTo(expectedId));
        
        // Verify the service was called once
        _mockPeopleService.Verify(s => s.CreatePersonAsync(It.IsAny<IncomingCreatePersonDTO>()), Times.Once);
    }

    [Test]
    public async Task UpdatePerson_WithExistingId_ReturnsNoContent()
    {
        // Arrange
        var personId = Guid.NewGuid();
        var updateDto = new IncomingUpdatePersonDTO(
            FirstName: "Updated",
            LastName: "Person",
            Email: "updated.person@example.com",
            Role: ApplicationRoles.Employee
        );
        
        // Use It.IsAny for object comparison
        _mockPeopleService
            .Setup(s => s.UpdatePersonAsync(personId, It.IsAny<IncomingUpdatePersonDTO>()))
            .ReturnsAsync(OperationResult.SuccessResult);
    
        // Act
        var result = await _controller.UpdatePerson(personId, updateDto);
    
        // Assert
        Assert.That(result, Is.InstanceOf<NoContentResult>());
        
        // Verify the service was called once with the correct ID
        _mockPeopleService.Verify(s => s.UpdatePersonAsync(personId, It.IsAny<IncomingUpdatePersonDTO>()), Times.Once);
    }
    
    [Test]
    public async Task UpdatePerson_WithNonexistentId_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var updateDto = new IncomingUpdatePersonDTO(
            FirstName: "Updated",
            LastName: "Person",
            Email: "updated.person@example.com",
            Role: ApplicationRoles.Employee
        );
        
        // Use It.IsAny for object comparison
        _mockPeopleService
            .Setup(s => s.UpdatePersonAsync(nonExistentId, It.IsAny<IncomingUpdatePersonDTO>()))
            .ReturnsAsync(OperationResult.FailureResult());

        // Act
        var result = await _controller.UpdatePerson(nonExistentId, updateDto);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundResult>());
        
        // Verify the service was called once with the correct ID
        _mockPeopleService.Verify(s => s.UpdatePersonAsync(nonExistentId, It.IsAny<IncomingUpdatePersonDTO>()), Times.Once);
    }

    [Test]
    public async Task DeletePerson_WithExistingId_ReturnsNoContent()
    {
        // Arrange
        var personId = Guid.NewGuid();
        _mockPeopleService.Setup(s => s.DeletePersonAsync(personId)).ReturnsAsync(OperationResult.SuccessResult);

        // Act
        var result = await _controller.DeletePerson(personId);

        // Assert
        Assert.That(result, Is.InstanceOf<NoContentResult>());
        
        // Verify the service was called once with the correct ID
        _mockPeopleService.Verify(s => s.DeletePersonAsync(personId), Times.Once);
    }

    [Test]
    public async Task DeletePerson_WithNonexistentId_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        _mockPeopleService.Setup(s => s.DeletePersonAsync(nonExistentId)).ReturnsAsync(OperationResult.FailureResult());

        // Act
        var result = await _controller.DeletePerson(nonExistentId);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundResult>());
        
        // Verify the service was called once with the correct ID
        _mockPeopleService.Verify(s => s.DeletePersonAsync(nonExistentId), Times.Once);
    }
}
