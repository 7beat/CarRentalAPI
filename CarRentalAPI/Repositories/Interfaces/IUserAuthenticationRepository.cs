﻿using CarRentalAPI.Models.DTO;
using CarRentalAPI.Models.Identity;
using Microsoft.AspNetCore.Identity;

namespace CarRentalAPI.Repositories.Interfaces
{
    public interface IUserAuthenticationRepository
    {
        Task<IdentityResult> RegisterUserAsync(UserRegistrationDto user);
    }
}