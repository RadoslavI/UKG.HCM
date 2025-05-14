namespace UKG.HCM.PeopleManagementApi.DTOs.Person.Get;

public record OutgoingGetPersonDTO(Guid Id, string FirstName, string LastName, string Email, RoleDTO Role);