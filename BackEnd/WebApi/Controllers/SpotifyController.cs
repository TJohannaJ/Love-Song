using Backend.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Backend.Domain.Interfaces;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SpotifyController : ControllerBase
    {
        private readonly ISpotifyService _spotifyService;
        private readonly ILogger<SpotifyController> _logger;
        private readonly IConfiguration _configuration;

        public SpotifyController(ISpotifyService spotifyService, ILogger<SpotifyController> logger, IConfiguration configuration)
        {
            _spotifyService = spotifyService;
            _logger = logger;
            _configuration = configuration;
        }

        [Authorize]
        [HttpGet("login-url")]
        public IActionResult GetLoginUrl()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var url = _spotifyService.GetLoginUrl(userId);
            return Ok(new { url });
        }

        [AllowAnonymous]
        [HttpGet("callback")]
        public async Task<IActionResult> Callback([FromQuery] string code, [FromQuery] string state)
        {
            var success = await _spotifyService.ExchangeCodeForTokensAsync(code, state);

            string script = success
                ? "<script>window.opener.postMessage('spotify-success', '*'); window.close();</script>"
                : "<script>window.opener.postMessage('spotify-error', '*'); window.close();</script>";

            return Content(script, "text/html");
        }

        [Authorize]
        [HttpGet("top-tracks")]
        public async Task<IActionResult> GetTopTracks()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var tracks = await _spotifyService.GetSavedUserTopSongsAsync(userId);
                return Ok(tracks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Hiba a mentett dalok lekérése során: {UserId}", userId);
                return StatusCode(500, new { message = "Hiba a mentett dalok lekérése során." });
            }
        }
    }
}