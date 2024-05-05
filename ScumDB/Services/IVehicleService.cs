using ScumDB.Models;
using ScumDB.Models.Enums;

namespace ScumDB.Services;

public interface IVehicleService
{
    public Task<List<VehicleModel>> GetAllAsync(bool bindOwner = true);
    public Task<List<VehicleModel>> GetRelatedOfAsync(List<int> vehicleIds);
    public Task UpdateVehiclesLocationAsync(List<VehicleModel> vehicles);
    public Task<int> AddAsync(List<VehicleModel> vehicles);
    public Task PurgeAsync(PurgeType purgeType, Func<VehicleModel, bool>? filters);
    public Task<List<VehicleModel>> ParseAsync(string content);
}