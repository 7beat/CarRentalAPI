﻿using AutoMapper;
using CarRentalAPI.Repositories;
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
        private readonly IMapper mapper;
        
        public VehiclesController(Repositories.IVehicleRepository vehicleRepository, IMapper mapper)
        {
            this.vehicleRepository = vehicleRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllVehicles()
        {
            var vehiclesDomain = await vehicleRepository.GetAllAsync();

            var vehiclesDTO = mapper.Map<List<Models.DTO.Vehicle>>(vehiclesDomain);

            return Ok(vehiclesDTO);
        }

        [HttpGet]
        [Route("{id:int}")]
        [ActionName("GetVehicleById")]
        public async Task<IActionResult> GetVehicleById(int id)
        {
            var vehicleDomain = await vehicleRepository.GetByIdAsync(id);

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
        public async Task<IActionResult> UpdateVehicleAsync([FromRoute] int id,
            [FromBody] Models.DTO.UpdateVehicleRequest updateVehicleRequest)
        {
            if (!ValidateUpdateVehicle(updateVehicleRequest))
                return BadRequest(ModelState);

            var vehicleDomain = new Models.Domain.Vehicle
            {
                Model = updateVehicleRequest.Model,
                ColorId = updateVehicleRequest.Color,
            };

            vehicleDomain = await vehicleRepository.UpdateAsync(id, vehicleDomain);

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
