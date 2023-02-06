﻿using CarRentalAPI.Data;
using CarRentalAPI.Models.Domain;
using CarRentalAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarRentalAPI.Repositories
{
    public class VehicleRepository : RepositoryBase<Vehicle>, IVehicleRepository
    {
        public VehicleRepository(AppDbContext appDbContext) : base(appDbContext)
        {

        }

        public async Task<Vehicle> AddAsync(Vehicle vehicle)
        {
            //var addedVehicle = await _appDbContext.Vehicles.AddAsync(vehicle);
            //await _appDbContext.SaveChangesAsync();
            await CreateAsync(vehicle); //Create nic nie zwraca dlatego ma nulla a id nie zostało zaakutalizowane
            //WIP
            return await GetByIdAsync(vehicle.Id);
        }

        public async Task<Vehicle> DeteleAsync(int id)
        {
            //var existingVehicle = await _appDbContext.Vehicles.FindAsync(id);
            var existingVehicle = await FindByConditionAsync(x => x.Id == id, true);
            var result = await existingVehicle.FirstOrDefaultAsync();

            if (result is not null)
            {
                await RemoveAsync(result);

                return result;
            }
            return null;
        }

        public async Task<IEnumerable<Vehicle>> GetAllAsync()
        {
            var result = await base.FindAllAsync(false);
            return await result.Include(x => x.Color).ToListAsync();
        }

        public async Task<Vehicle> GetByIdAsync(int id)
        //=> await _appDbContext.Vehicles
        //    .Include(x => x.Color)
        //    .FirstOrDefaultAsync(x => x.Id == id);
        {
            var query = await base.FindByConditionAsync(x => x.Id.Equals(id), false);
            var result = await query.Include(x => x.Color).FirstOrDefaultAsync();

            return result;
        }

        public async Task<Vehicle> UpdateAsync(int id, Vehicle vehicle)
        {
            var existingVehicle = await FindByConditionAsync(x => x.Id.Equals(id), true);
            var result = await existingVehicle.FirstOrDefaultAsync();


            if (existingVehicle is null)
                return null;

            ////Modification
            //existingVehicle.ColorId = vehicle.ColorId;
            //existingVehicle.Model = vehicle.Model;
            //existingVehicle.Model = vehicle.Model;

            //await _appDbContext.SaveChangesAsync();
            return result; //???
        }
    }
}
