﻿using AutoMapper;
using CarRentalAPI.Data;
using CarRentalAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace CarRentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehiclesController : ControllerBase
    {
        private readonly IVehicleRepository vehicleRepository;
        private readonly IVehicle2Repository vehicle2Repository;
        private readonly IRepositoryManager repository;
        private readonly IMapper mapper;
        public VehiclesController(IVehicleRepository vehicleRepository, IMapper mapper, IVehicle2Repository vehicle2Repository, IRepositoryManager repository)
        {
            this.vehicleRepository = vehicleRepository;
            this.mapper = mapper;
            this.vehicle2Repository = vehicle2Repository;
            this.repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllVehicles()
        {
            //var vehiclesDomain = await vehicleRepository.GetAllAsync();
            //var vehiclesDomain = await vehicle2Repository.GetAllVehicles(false);
            var vehiclesDomain = await repository.Vehicle.GetAllVehicles(false);
            Console.WriteLine();
            var vehiclesDTO = mapper.Map<IEnumerable<Models.DTO.Vehicle>>(vehiclesDomain);

            return Ok(vehiclesDTO);
        }

        [HttpGet]
        [Route("{id:int}")]
        [ActionName("GetVehicleById")]
        public async Task<IActionResult> GetVehicleById(int id)
        {
            //var vehicleDomain = await vehicleRepository.GetByIdAsync(id);
            var vehicleDomain = await vehicle2Repository.GetVehicle(id, false);

            if (vehicleDomain is null)
                return NotFound();

            var vehicleDTO = mapper.Map<Models.DTO.Vehicle>(vehicleDomain);

            return Ok(vehicleDTO);
        }

        [HttpPost]
        public async Task<IActionResult> AddVehicle([FromBody] Models.DTO.AddVehicleRequest addVehicleRequest)
        {
            var vehicleDomain = new Models.Domain.Vehicle
            {
                Brand = addVehicleRequest.Brand,
                Model = addVehicleRequest.Model,
                ColorId = addVehicleRequest.Color,
                YearOfProduction = addVehicleRequest.YearOfProduction,
            };

            vehicleDomain = await vehicleRepository.AddAsync(vehicleDomain);

            var vehicleDTO = mapper.Map<Models.DTO.Vehicle>(vehicleDomain);

            //201
            return CreatedAtAction(nameof(GetVehicleById), new {id = vehicleDTO.Id}, vehicleDTO);
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> UpdateVehicleAsync(int id,
            [FromBody] Models.DTO.UpdateVehicleRequest updateVehicleRequest)
        {
            if (!ValidateUpdateVehicle(updateVehicleRequest))
                return BadRequest(ModelState);

            //var vehicleDomain = new Models.Domain.Vehicle
            //{
            //    Model = updateVehicleRequest.Model,
            //    ColorId = updateVehicleRequest.Color,
            //};

            //vehicleDomain = await vehicleRepository.UpdateAsync(id, vehicleDomain);

            // New
            // Vehicles .GetById
            var vehicleDomain = await repository.Vehicle.GetVehicle(id, true);

            //if (vehicleDomain is null)
            //    return NotFound();

            //// New better
            //mapper.Map(updateVehicleRequest, vehicleDomain);
            //await repository.Vehicle.UpdateVehicle(vehicleDomain);
            //await repository.SaveAsync();

            //var vehicleDTO = mapper.Map<Models.DTO.Vehicle>(vehicleDomain);
            //return Ok(vehicleDTO);

            // New 
            vehicleDomain.Model = updateVehicleRequest.Model;
            vehicleDomain.ColorId = updateVehicleRequest.Color;

            await repository.Vehicle.UpdateVehicle(vehicleDomain);
            await repository.SaveAsync();

            var vehicleDTO = mapper.Map<Models.DTO.Vehicle>(vehicleDomain);

            return Ok(vehicleDTO);
            //return CreatedAtAction(nameof(GetVehicleById), new { id = vehicleDTO.Id }, vehicleDTO);
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteVehicleAsync(int id)
        {
            //var vehicleDomain = await vehicleRepository.DeteleAsync(id);
            var vehicleDomain = await vehicle2Repository.GetVehicle(id, true);

            //Deleting vehicle
            await vehicle2Repository.DeleteVehicle(vehicleDomain);
            await repository.SaveAsync();

            if (vehicleDomain is null)
                return NotFound();

            var vehicleDTO = mapper.Map<Models.DTO.Vehicle>(vehicleDomain);
            return Ok(vehicleDTO);
        }

        private bool ValidateUpdateVehicle(Models.DTO.UpdateVehicleRequest updateVehicleRequest)
        {
            if (updateVehicleRequest is null)
            {
                ModelState.AddModelError(nameof(updateVehicleRequest), $"{nameof(updateVehicleRequest)} cant be empty.");
                return false;
            }

            if (updateVehicleRequest.Color <= 0)
            {
                ModelState.AddModelError(nameof(updateVehicleRequest), $"{nameof(updateVehicleRequest.Color)} needs to be specified.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(updateVehicleRequest.Model))
            {
                ModelState.AddModelError(nameof(updateVehicleRequest), $"{nameof(updateVehicleRequest.Model)} is required.");
                return false;
            }

            //return ModelState.ErrorCount <= 0;
            //return ModelState.ErrorCount > 0 ? false : true;
            if (ModelState.ErrorCount > 0)
            {
                return false;
            }

            return true;
        }
    }
}
