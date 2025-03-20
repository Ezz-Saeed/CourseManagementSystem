﻿using APIs.DTOs;
using APIs.DTOs.TrainerDtos;
using APIs.Models;
using System.IdentityModel.Tokens.Jwt;

namespace APIs.Interfaces
{
    public interface IAuthService
    {
        Task<AuthDto> RegisterAsync(RegisterDto dto);
        Task<AuthDto> GetTokenAsync(LoginDto dto);
        Task<AuthDto> RefreshTokenAsync(string refreshToken);
        //Task<AppUser> LoadCurrentUser(string userId);
        Task<JwtSecurityToken> GenerateToken(Appuser appUser);
        Task<ResponseDto> UpdateTrainerAsync(UpdateTrainerDto dto, string id);
        Task<ResponseDto> DeleteTrainerAsync(string id);
        Task<List<GetTrainerDto>> GetTrainersAsync();
    }
}
