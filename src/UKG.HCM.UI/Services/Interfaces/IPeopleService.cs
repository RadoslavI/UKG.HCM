using UKG.HCM.UI.Models;

namespace UKG.HCM.UI.Services.Interfaces;

/// <summary>
/// Service for interacting with the People Management API
/// </summary>
public interface IPeopleService
{
    /// <summary>
    /// Gets all people
    /// </summary>
    /// <returns>Collection of person records</returns>
    Task<IEnumerable<PersonViewModel>> GetPeopleAsync();
    
    /// <summary>
    /// Gets a person by ID
    /// </summary>
    /// <param name="id">Person ID</param>
    /// <returns>Person record if found, null otherwise</returns>
    Task<PersonViewModel?> GetPersonByIdAsync(Guid id);
    
    /// <summary>
    /// Creates a new person
    /// </summary>
    /// <param name="person">Person data</param>
    /// <returns>ID of the created person</returns>
    Task<Guid> CreatePersonAsync(CreatePersonViewModel person);
    
    /// <summary>
    /// Updates an existing person
    /// </summary>
    /// <param name="id">Person ID</param>
    /// <param name="person">Updated person data</param>
    /// <returns>True if updated successfully, false otherwise</returns>
    Task<(bool Success, string ErrorMessage)> UpdatePersonAsync(Guid id, UpdatePersonViewModel person);
    
    /// <summary>
    /// Deletes a person
    /// </summary>
    /// <param name="id">Person ID</param>
    /// <returns>True if deleted successfully, false otherwise</returns>
    Task<bool> DeletePersonAsync(Guid id);
}
