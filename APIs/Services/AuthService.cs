using APIs.DTOs;
using APIs.Helpers;
using APIs.Interfaces;
using APIs.Models;
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
        IOptions<JWT> jwtOptions, RoleManager<IdentityRole> roleManager) : IAuthService
    {
        private readonly JWT jwt = jwtOptions.Value;

        public async Task<AuthDto> RegisterAsync(RegisterDto model)
        {
            if (await userManager.FindByEmailAsync(model.Email) is not null ||
                await userManager.FindByNameAsync(model.UserName) is not null)
                return new AuthDto { Message = "Already registered account!" };
            var user = new Appuser
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
            };

            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errors = string.Empty;
                foreach (var error in result.Errors)
                    errors += $"{error.Description}, ";
                return new AuthDto { Message = errors };
            }

            await userManager.AddToRoleAsync(user, "User");
            var token = await GenerateToken(user);
            return new AuthDto
            {
                Email = user.Email,
                Username = user.Email,
                Roles = new List<string> { "User" },
                IsAuthenticated = true,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresOn = token.ValidTo,
                FirstName = user.FirstName,
                LastName = user.LastName
            };

        }


        public async Task<AuthDto> GetTokenAsync(LoginDto model)
        {
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

            if (user.RefreshTokens.Any(t => t.IsActive))
            {
                var activeRefreshToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);
                authModel.RefreshToken = activeRefreshToken.Token;
                authModel.RefreshTokenExpiration = activeRefreshToken.ExpiresOn;
            }
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
            var activeRefreshToken = user.RefreshTokens.Single(rt => rt.Token == refreshToken);

            if (!activeRefreshToken.IsActive)
            {
                authModel.Message = "Inactive refresh token!";
                return authModel;
            }
            activeRefreshToken.RevokedOn = DateTime.UtcNow;
            var newRefreshToken = GenerateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);
            await userManager.UpdateAsync(user);

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

        public async Task<Appuser> LoadCurrentUser(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return null!;
            var user = await userManager.FindByIdAsync(userId);
            if (user is null) return null!;
            return user;
        }

        public async Task<JwtSecurityToken> GenerateToken(Appuser appUser)
        {
            var userClaims = await userManager.GetClaimsAsync(appUser);
            var roles = await userManager.GetRolesAsync(appUser);
            var roleClaims = new List<Claim>();
            foreach (var role in roles)
            {
                roleClaims.Add(new Claim("roles", role));
            }

            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, appUser.UserName!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, appUser.Email!),
                new Claim(JwtRegisteredClaimNames.NameId, appUser.Id)
            }.Union(userClaims).Union(roleClaims);

            var signinKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key));
            var signinCredentials = new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256);

            return new JwtSecurityToken(
                    issuer: jwt.Issuer,
                    audience: jwt.Audience,
            claims: claims,
                    signingCredentials: signinCredentials,
                    expires: DateTime.Now.AddDays(jwt.DurationInMinutes)
                );
        }


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
