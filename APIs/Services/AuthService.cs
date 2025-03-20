using APIs.DTOs;
using APIs.DTOs.TrainerDtos;
using APIs.Helpers;
using APIs.Interfaces;
using APIs.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace APIs.Services
{
    public class AuthService(UserManager<Appuser> userManager,
        IOptions<JWT> jwtOptions, IMapper mapper) : IAuthService
    {
        private readonly JWT jwt = jwtOptions.Value;

        // A method service to register new app user with trainer role
        public async Task<AuthDto> RegisterAsync(RegisterDto model)
        {
            // Registered account
            if (await userManager.FindByEmailAsync(model.Email) is not null ||
                await userManager.FindByNameAsync(model.UserName) is not null)
                return new AuthDto { Message = "Already registered account!" };

            // Create new app user
            var user = new Appuser
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
            };

            // User registeration succeeded
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errors = string.Empty;
                foreach (var error in result.Errors)
                    errors += $"{error.Description}, ";
                return new AuthDto { Message = errors };
            }

            await userManager.AddToRoleAsync(user, "Trainer");
            var token = await GenerateToken(user);
            return new AuthDto
            {
                Email = user.Email,
                Username = user.Email,
                Roles = new List<string> { "Trainer" },
                IsAuthenticated = true,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresOn = token.ValidTo,
                FirstName = user.FirstName,
                LastName = user.LastName
            };

        }

        // A method to get JWT token so that user is authenticated
        public async Task<AuthDto> GetTokenAsync(LoginDto model)
        {
            // Unauthenticated user
            var authModel = new AuthDto();
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user is null || !await userManager.CheckPasswordAsync(user, model.Password))
            {
                authModel.Message = "Invalid email or password!";
                return authModel;
            }
            var token = await GenerateToken(user);
            var roles = await userManager.GetRolesAsync(user);

            authModel.Username = user.UserName;
            authModel.Email = user.Email;
            authModel.Roles = roles.ToList();
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(token);
            authModel.ExpiresOn = token.ValidTo.ToLocalTime();
            authModel.FirstName = user.FirstName;
            authModel.LastName = user.LastName;
            authModel.IsAuthenticated = true;

            // Check for active refresh token
            if (user.RefreshTokens.Any(t => t.IsActive))
            {
                var activeRefreshToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);
                authModel.RefreshToken = activeRefreshToken.Token;
                authModel.RefreshTokenExpiration = activeRefreshToken.ExpiresOn;
            }
            // If not generate new one
            else
            {
                var refreshToken = GenerateRefreshToken();
                authModel.RefreshToken = refreshToken.Token;
                authModel.RefreshTokenExpiration = refreshToken.ExpiresOn;
                user.RefreshTokens.Add(refreshToken);
                await userManager.UpdateAsync(user);
            }
;
            return authModel;
        }

        // Refresh expired jwt token
        public async Task<AuthDto> RefreshTokenAsync(string refreshToken)
        {
            var authModel = new AuthDto();
            var user = await userManager.Users.SingleOrDefaultAsync
                (u => u.RefreshTokens.Any(rt => rt.Token == refreshToken));
            if (user is null)
            {
                authModel.Message = "Invalid refresh token!";
                return authModel;
            }
            // Fetch an active refresh token
            var activeRefreshToken = user.RefreshTokens.Single(rt => rt.Token == refreshToken);

            if (!activeRefreshToken.IsActive)
            {
                authModel.Message = "Inactive refresh token!";
                return authModel;
            }
            // Revoke cuurent refresh token to generate new active one
            activeRefreshToken.RevokedOn = DateTime.UtcNow;
            var newRefreshToken = GenerateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);
            await userManager.UpdateAsync(user);

            // Generate new JWT token
            var jwtToken = await GenerateToken(user);
            var roles = await userManager.GetRolesAsync(user);
            authModel.Email = user.Email;
            authModel.Username = user.UserName;
            authModel.Roles = roles.ToList();
            authModel.IsAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            authModel.RefreshToken = newRefreshToken.Token;
            authModel.RefreshTokenExpiration = newRefreshToken.ExpiresOn;
            return authModel;
        }

        public async Task<List<GetTrainerDto>> GetTrainersAsync()
        {
            var trainers = await userManager.Users.Where(u=>!u.IsDeleted).ToListAsync();
            var returnedTrainers = mapper.Map<List<GetTrainerDto>>(trainers);
            return returnedTrainers;
        }


        // Edit trainer account
        public async Task<ResponseDto> UpdateTrainerAsync(UpdateTrainerDto dto, string id)
        {
            var result = new ResponseDto();
            var user = await userManager.FindByIdAsync(id);
            // Check for unoauthorized user
            if(user is null)
            {
                result.StatusCode = 401;
                result.Message = "Unauthorized user!";
                return result;
            }

            // Check for new info to be updated
            if(!string.IsNullOrEmpty(dto.UserName))
                user.UserName = dto.UserName;

            if (!string.IsNullOrEmpty(dto.Email))
                user.Email = dto.Email;

            if (!string.IsNullOrEmpty(dto.FirstName))
                user.FirstName = dto.FirstName;

            if (!string.IsNullOrEmpty(dto.LastName))
                user.LastName = dto.LastName;

            var updateResult = await userManager.UpdateAsync(user!);
            // Check for update errors
            if(!updateResult.Succeeded)
            {
                result.StatusCode = 400;
                result.Message = $"{string.Join(", ", updateResult.Errors.Select(e=>e.Description))}";
                return result;
            }
            result.StatusCode = 200;
            result.Message = "Triner info updated successfully";

            return result;
        }


        public async Task<ResponseDto> DeleteTrainerAsync(string id)
        {
            var result = new ResponseDto();
            var user = await userManager.FindByIdAsync(id);
            // Check for unoauthorized user
            if (user is null)
            {
                result.StatusCode = 401;
                result.Message = "Unauthorized user!";
                return result;
            }
            
            user.IsDeleted = true;
            var updateResult = await userManager.UpdateAsync(user);

            // Check for update errors
            if (!updateResult.Succeeded)
            {
                result.StatusCode = 400;
                result.Message = $"{string.Join(", ", updateResult.Errors.Select(e => e.Description))}";
                return result;
            }
            result.StatusCode = 200;
            result.Message = "Triner deleted successfully";

            return result;
        }

        // Method to generate JWT token object for app user
        public async Task<JwtSecurityToken> GenerateToken(Appuser appUser)
        {
            // Extracting roles and claims of app user
            var userClaims = await userManager.GetClaimsAsync(appUser);
            var roles = await userManager.GetRolesAsync(appUser);
            var roleClaims = new List<Claim>();
            foreach (var role in roles)
            {
                roleClaims.Add(new Claim("roles", role));
            }

            // Claims for user
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, appUser.UserName!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, appUser.Email!),
                new Claim(JwtRegisteredClaimNames.NameId, appUser.Id)
            }.Union(userClaims).Union(roleClaims);

            var signinKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key));
            var signinCredentials = new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256);

            // Token generation
            return new JwtSecurityToken(
                    issuer: jwt.Issuer,
                    audience: jwt.Audience,
                    claims: claims,
                    signingCredentials: signinCredentials,
                    expires: DateTime.Now.AddDays(jwt.DurationInMinutes)
                );
        }

        // Method to generate RefreshToken object
        private RefreshToken GenerateRefreshToken()
        {
            var randomNumber = new byte[32];

            using var generator = new RNGCryptoServiceProvider();
            generator.GetBytes(randomNumber);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                CreatedOn = DateTime.UtcNow,
                ExpiresOn = DateTime.UtcNow.AddDays(3),
            };
        }


    }
}
