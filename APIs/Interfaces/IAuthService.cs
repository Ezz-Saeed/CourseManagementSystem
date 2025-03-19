using APIs.DTOs;
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
    }
}
