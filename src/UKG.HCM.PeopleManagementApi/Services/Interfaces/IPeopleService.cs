using UKG.HCM.PeopleManagementApi.DTOs.Person.Create;
using UKG.HCM.PeopleManagementApi.DTOs.Person.Get;
using UKG.HCM.PeopleManagementApi.DTOs.Person.Update;

namespace UKG.HCM.PeopleManagementApi.Services.Interfaces;

public interface IPeopleService
{
    Task<IEnumerable<OutgoingGetPersonDTO>> GetPeopleAsync();
    Task<OutgoingGetPersonDTO?> GetPersonByIdAsync(Guid id);
    Task<Guid> CreatePersonAsync(IncomingCreatePersonDTO incoming);
    Task<bool> UpdatePersonAsync(Guid id, IncomingUpdatePersonDTO updatedPerson);
    Task<bool> DeletePersonAsync(Guid id);
}