using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using UKG.HCM.PeopleManagementApi.Data;
using UKG.HCM.PeopleManagementApi.Data.Entities;
using UKG.HCM.PeopleManagementApi.DTOs.Person.Create;
using UKG.HCM.PeopleManagementApi.DTOs.Person.Get;
using UKG.HCM.PeopleManagementApi.DTOs.Person.Update;
using UKG.HCM.Shared.Constants;

namespace UKG.HCM.PeopleManagementApi.Tests.Integration;

[TestFixture]
public class PeopleControllerIntegrationTests
{
    private TestWebApplicationFactory<Program> _factory;
    private HttpClient _client;

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
    public async Task GetPeople_ReturnsAllPeople()
    {
        // Arrange
        await SeedPeopleData();

        // Act
        var response = await _client.GetAsync("/api/people");
        
        // Assert
        response.EnsureSuccessStatusCode();
        
        var people = await response.Content.ReadFromJsonAsync<IEnumerable<OutgoingGetPersonDTO>>();
        Assert.That(people, Is.Not.Null);
        Assert.That(people.Count(), Is.EqualTo(3));
    }

    [Test]
    public async Task GetPerson_WithValidId_ReturnsPerson()
    {
        // Arrange
        var personId = await SeedPersonAndGetId();

        // Act
        var response = await _client.GetAsync($"/api/people/{personId}");
        
        // Assert
        response.EnsureSuccessStatusCode();
        
        var person = await response.Content.ReadFromJsonAsync<OutgoingGetPersonDTO>();
        Assert.That(person, Is.Not.Null);
        Assert.That(person.Id, Is.EqualTo(personId));
    }

    [Test]
    public async Task GetPerson_WithInvalidId_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync($"/api/people/{Guid.NewGuid()}");
        
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task CreatePerson_WithValidData_ReturnsCreated()
    {
        // Arrange
        var dto = new IncomingCreatePersonDTO(
            FirstName: "John",
            LastName: "Doe",
            Email: "john.doe@example.com",
            Role: ApplicationRoles.Employee,
            HireDate: DateTime.Now
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/people", dto);
        
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        
        var createdId = await response.Content.ReadFromJsonAsync<Guid>();
        Assert.That(createdId, Is.Not.EqualTo(Guid.Empty));

        // Verify the person was actually created
        var getResponse = await _client.GetAsync($"/api/people/{createdId}");
        getResponse.EnsureSuccessStatusCode();
    }

    [Test]
    public async Task UpdatePerson_WithValidData_ReturnsNoContent()
    {
        // Arrange
        var personId = await SeedPersonAndGetId();
        
        var updateDto = new IncomingUpdatePersonDTO(
            FirstName: "Updated",
            LastName: "Person",
            Email: "updated.person@example.com",
            Role: ApplicationRoles.Manager
        );

        // Act
        var response = await _client.PutAsJsonAsync($"/api/people/{personId}", updateDto);
        
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

        // Verify the update was successful
        var getResponse = await _client.GetAsync($"/api/people/{personId}");
        getResponse.EnsureSuccessStatusCode();
        
        var updatedPerson = await getResponse.Content.ReadFromJsonAsync<OutgoingGetPersonDTO>();
        Assert.That(updatedPerson, Is.Not.Null);
        Assert.That(updatedPerson.FirstName, Is.EqualTo("Updated"));
        Assert.That(updatedPerson.LastName, Is.EqualTo("Person"));
        Assert.That(updatedPerson.Email, Is.EqualTo("updated.person@example.com"));
        Assert.That(updatedPerson.Role, Is.EqualTo(ApplicationRoles.Manager));
    }

    [Test]
    public async Task DeletePerson_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var personId = await SeedPersonAndGetId();

        // Act
        var response = await _client.DeleteAsync($"/api/people/{personId}");
        
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

        // Verify the person was deleted
        var getResponse = await _client.GetAsync($"/api/people/{personId}");
        Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    #region Helper Methods

    private async Task SeedPeopleData()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PeopleContext>();

        // Clear any existing data
        context.People.RemoveRange(await context.People.ToListAsync());
        await context.SaveChangesAsync();

        // Add test data
        var people = new List<Person>
        {
            new()
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Role = Role.HRAdmin,
                CreatedAt = DateTime.UtcNow,
                HireDate = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@example.com",
                Role = Role.Manager,
                CreatedAt = DateTime.UtcNow,
                HireDate = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                FirstName = "Bob",
                LastName = "Johnson",
                Email = "bob.johnson@example.com",
                Role = Role.Employee,
                CreatedAt = DateTime.UtcNow,
                HireDate = DateTime.UtcNow
            }
        };

        await context.People.AddRangeAsync(people);
        await context.SaveChangesAsync();
    }

    private async Task<Guid> SeedPersonAndGetId()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PeopleContext>();

        var person = new Person
        {
            Id = Guid.NewGuid(),
            FirstName = "Test",
            LastName = "Person",
            Email = "test.person@example.com",
            Role = Role.Employee,
            CreatedAt = DateTime.UtcNow,
            HireDate = DateTime.UtcNow
        };

        await context.People.AddAsync(person);
        await context.SaveChangesAsync();

        return person.Id;
    }

    #endregion
}
