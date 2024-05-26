using Microsoft.EntityFrameworkCore;
using ScumDB.Databases;
using ScumDB.Extensions;
using ScumDB.Models;
using ScumDB.Models.Enums;

namespace ScumDB.Services;

public class VehicleService(IDbContextFactory<ScumDbContext> factory) : IVehicleService
{

    public async Task<List<VehicleModel>> GetAllAsync(bool bindOwner = true)
    {
        var db = await factory.CreateDbContextAsync();
        var vehicles = await db.Vehicles.AsNoTracking().ToListAsync();
        if (bindOwner)
        {
            var accounts = await db.Accounts.AsNoTracking().ToListAsync();
            foreach (var vehicle in vehicles)
            {
                vehicle.OwnerName = accounts.FirstOrDefault(x => x.SteamId == vehicle.OwnerId)?.Name;
            }
        }
        await db.DisposeAsync();

        return vehicles;
    }
    
    /// <inheritdoc/>
    public async Task<List<VehicleModel>> GetVehiclesByIds(List<int> vehicleIds)
    {
        var db = await factory.CreateDbContextAsync();
        var vehicles = db.Vehicles.AsNoTracking().Where(x => vehicleIds.Contains(x.VehicleId)).ToList();
        await db.DisposeAsync();
        return vehicles;
    }
    
    /// <inheritdoc/>
    public async Task<int> UpdateVehiclesLocationAsync(List<VehicleModel> vehicles)
    {
        var vehiclesToUpdate = await GetVehiclesByIds(vehicles.Select(x => x.VehicleId).ToList());
        foreach (var vehicle in vehiclesToUpdate)
        {
            var newLocation = vehicles.FirstOrDefault(x => x.VehicleId == vehicle.VehicleId);
            if (newLocation != null)
            {
                vehicle.PositionX = newLocation.PositionX;
                vehicle.PositionY = newLocation.PositionY;
                vehicle.PositionZ = newLocation.PositionZ;
            }
        }

        var db = await factory.CreateDbContextAsync();
        db.Vehicles.UpdateRange(vehiclesToUpdate);
        await db.SaveChangesAsync();
        await db.DisposeAsync();

        return vehiclesToUpdate.Count;
    }

    /// <inheritdoc/>
    public async Task<int> TryAddAsync(List<VehicleModel> vehicles)
    {
        var db = await factory.CreateDbContextAsync();
        var vehiclesToAdd = vehicles.Where(x => db.Vehicles.All(y => y.VehicleId != x.VehicleId)).ToList();
        db.Vehicles.AddRange(vehiclesToAdd);

        await db.SaveChangesAsync();
        await db.DisposeAsync();

        return vehiclesToAdd.Count;
    }
    
    /// <summary>
    /// Purges the vehicles table according to the given purging type and the optional filters.
    /// </summary>
    /// <param name="purgeType">The purge type <see cref="PurgeType"/> between Soft and Hard</param>
    /// <param name="filters">Filters used to select which vehicles to remove from DB (optional)</param>
    public async Task PurgeAsync(PurgeType purgeType, Func<VehicleModel, bool>? filters)
    {
        var db = await factory.CreateDbContextAsync();
        List<VehicleModel> toDell = [];
        
        switch (purgeType)
        {
            case PurgeType.Soft:
            {
                toDell = db.Vehicles.AsNoTracking().Where(x => string.IsNullOrWhiteSpace(x.OwnerId) || !x.OwnerId.StartsWith("7")).ToList();
                break;
            }

            case PurgeType.Hard:
            {
                if (filters != null) toDell = db.Vehicles.AsNoTracking().Where(filters).ToList();
                else toDell = await db.Vehicles.AsNoTracking().ToListAsync();
                
                break;
            }
        }

        db.Vehicles.RemoveRange(toDell);
        await db.SaveChangesAsync();
        await db.DisposeAsync();
    }

    /// <inheritdoc/>
    public Task<List<VehicleModel>> ParseAsync(string content)
    {
        List<VehicleModel> vehicles = [];
		
        var lines = content.Split("#", StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim());
        foreach (var line in lines)
        {
            var data = line.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
            if (data.Count() >= 8 && data[7].StartsWith("7"))
            {
                try
                {
                    vehicles.Add(new VehicleModel
                    {
                        Name = Hardcoded.GetVehicleName(data[1]),
                        Blueprint = data[1],
                        OwnerId = data[7],
                        VehicleId = int.Parse(data[0].Replace(":", string.Empty)),
                        PositionX = data[3].Replace("X=", string.Empty),
                        PositionY = data[4].Replace("Y=", string.Empty),
                        PositionZ = data[5].Replace("Z=", string.Empty),
                    });
                }
                catch (Exception)
                {
                    continue;
                }
            }
        }

        return Task.FromResult(vehicles);
    }
}