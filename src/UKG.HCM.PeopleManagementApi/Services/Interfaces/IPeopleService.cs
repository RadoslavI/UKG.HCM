using UKG.HCM.PeopleManagementApi.DTOs.Person.Create;
using UKG.HCM.PeopleManagementApi.DTOs.Person.Get;
using UKG.HCM.PeopleManagementApi.DTOs.Person.Update;
using UKG.HCM.Shared.Utilities;

namespace UKG.HCM.PeopleManagementApi.Services.Interfaces;

public interface IPeopleService
{
    Task<IEnumerable<OutgoingGetPersonDTO>> GetPeopleAsync();
    Task<OutgoingGetPersonDTO?> GetPersonByIdAsync(Guid id);
    Task<Guid?> CreatePersonAsync(IncomingCreatePersonDTO incoming);
    Task<OperationResult> UpdatePersonAsync(Guid id, IncomingUpdatePersonDTO updatedPerson);
    Task<OperationResult> DeletePersonAsync(Guid id);
}