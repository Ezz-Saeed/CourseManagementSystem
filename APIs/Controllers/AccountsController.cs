using API.Extensions;
using APIs.DTOs;
using APIs.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController(IAuthService authenticationService) : ControllerBase
    {
        [HttpGet("getTrainers")]
        public async Task<IActionResult> GetTrainers()
        {
            var trainers = await authenticationService.GetTrainersAsync();
            return Ok(trainers);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await authenticationService.RegisterAsync(model);

            if (!result.IsAuthenticated) return Unauthorized(result);
            if (!string.IsNullOrEmpty(result.RefreshToken))
                SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);

            return Ok(result);
        }

        [HttpPost("getToken")]
        public async Task<ActionResult> GetTokenAsnc(LoginDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await authenticationService.GetTokenAsync(model);
            if (!result.IsAuthenticated) return Unauthorized(result);

            if (!string.IsNullOrEmpty(result.RefreshToken))
                SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);

            return Ok(result);
        }

        [HttpGet("refreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if(string.IsNullOrEmpty(refreshToken))
                return Unauthorized();
            var result = await authenticationService.RefreshTokenAsync(refreshToken);

            if (!result.IsAuthenticated)
                return Unauthorized(result.Message);
            SetRefreshTokenInCookie(result.RefreshToken!, result.RefreshTokenExpiration);
            return Ok(result);
        }

        [Authorize(Roles = "Trainer")]
        [HttpPut("updateTrainer")]
        public async Task<IActionResult> UpdateTrainer(UpdateTrainerDto dto)
        {
            var id = User.GetUserId();
            var result =await authenticationService.UpdateTrainerAsync(dto, id);
            if (result.StatusCode == 401)
                return Unauthorized(result);
            if (result.StatusCode == 400)
                return BadRequest(result);
            
            return Ok(result);
        }

        [Authorize(Roles = "Trainer")]
        [HttpPut("deleteTrainer")]
        public async Task<IActionResult> DeleteTrainer()
        {
            var id = User.GetUserId();
            var result = await authenticationService.DeleteTrainerAsync(id);
            return Ok(result);
        }

        private void SetRefreshTokenInCookie(string refreshToken, DateTime expires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = expires.ToLocalTime(),
                Secure = true,
                SameSite = SameSiteMode.Strict,
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
    }
}
