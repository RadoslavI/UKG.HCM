namespace UKG.HCM.PeopleManagementApi.DTOs.Person.Update;

public record IncomingUpdatePersonDTO(string FirstName, string LastName, string Email, RoleDTO Role);