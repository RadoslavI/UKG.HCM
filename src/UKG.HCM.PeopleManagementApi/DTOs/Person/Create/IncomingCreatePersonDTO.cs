namespace UKG.HCM.PeopleManagementApi.DTOs.Person.Create;

public record IncomingCreatePersonDTO(string FirstName, string LastName, string Email, RoleDTO Role);