using Azure.Core;
using Backend.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Backend.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpotifyController : ControllerBase
    {
        private readonly SpotifyService _spotifyService;

        public SpotifyController(SpotifyService spotifyService)
        {
            _spotifyService = spotifyService;
        }

        [AllowAnonymous]
        [HttpGet("callback")]
        public async Task<IActionResult> Callback([FromQuery] string code, [FromQuery] string state)
        {
            var userId = state;
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
                return Redirect("https://localhost:5175/profile?spotify=error&reason=missing_data");

            try
            {
                var success = await _spotifyService.ConnectSpotifyAsync(userId, code);
                if (success)
                {
                    return Redirect("https://localhost:5175/profile?spotify=success");
                }
                return Redirect("https://localhost:5175/profile?spotify=error&reason=sync_failed");
            }
            catch (Exception)
            {
                return Redirect("https://localhost:5175/profile?spotify=error&reason=exception");
            }
        }

        [Authorize]
        [HttpGet("login-url")]
        public async Task<IActionResult> GetLoginUrl() 
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var loginUrl = await _spotifyService.GetSpotifyLoginUrlAsync(userId);
                return Ok(new { url = loginUrl });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Nem sikerült generálni a Spotify belépési linket." });
            }
        }
    }
}
