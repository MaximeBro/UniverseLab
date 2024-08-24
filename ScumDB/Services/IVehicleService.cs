using ScumDB.Models;
using ScumDB.Models.Enums;

namespace ScumDB.Services;

public interface IVehicleService
{
    /// <summary>
    /// Retrieves all the <see cref="VehicleModel"/> database according to the given ids.
    /// <param name="bindOwner">If owner name should be retrieved and bound to each VehicleModel.</param>
    /// <returns>A list of <see cref="VehicleModel"/></returns>
    /// </summary>
    public Task<List<VehicleModel>> GetAllAsync(bool bindOwner = true);
    
    /// <summary>
    /// Adds the given vehicles to the database if they aren't yet registered.
    /// </summary>
    /// <param name="vehicles">A list of <see cref="VehicleModel"/></param>
    /// <returns>The count of successfully added vehicles</returns>
    public Task<int> AddAsync(params VehicleModel[] vehicles);
    
    /// <summary>
    /// Updates all the <see cref="VehicleModel"/>'s location from DB according to the given vehicles id and location.
    /// </summary>
    /// <param name="vehicles">A list of <see cref="VehicleModel"/></param>
    /// <returns>The count of successfully updated vehicles</returns>
    public Task<int> UpdateVehiclesLocationAsync(List<VehicleModel> vehicles);
    
    public Task PurgeAsync(PurgeType purgeType, Func<VehicleModel, bool>? filters);
    
    /// <summary>
    /// Parses the given string into a list of <see cref="VehicleModel"/>.
    /// </summary>
    /// <returns>A list of <see cref="VehicleModel"/></returns>
    /// <param name="content">The resulted content of the #ListSpawnedVehicles command in game</param>
    public Task<VehicleModel[]> ParseAsync(string content);
}