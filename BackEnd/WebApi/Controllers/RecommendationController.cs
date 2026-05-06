using Backend.Domain.DTOs;
using Backend.Domain.Services;
using Backend.WebApi.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Backend.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RecommendationController : Controller
    {
        private readonly DiscoveryService _discoveryService;
        private readonly MatchService _matchService;

        public RecommendationController(MatchService matchService, DiscoveryService discoveryService)
        {
            _matchService = matchService;
            _discoveryService = discoveryService;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<ProfileDTO>>> GetRecomenndationsForUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            try
            {                
                var recommendedUsers = await _discoveryService.GetRecommendationsForUser(userId);
                var recommendations = recommendedUsers.Select(u => u.ToProfileDTO()).ToList();
                return Ok(recommendations);
            }
            catch (UnauthorizedAccessException e)
            {
                return Unauthorized(new { message = e.Message });
            }
        }

        [Authorize]
        [HttpPost("like")]
        public async Task<IActionResult> LikeUser([FromBody] RequestDTO request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            if (request == null || string.IsNullOrEmpty(request.RequestedUserId))
            {
                return BadRequest("Érvénytelen kérés: hiányzó felhasználó ID.");
            }
            try
            {
                await _matchService.LikeUser(userId, request.RequestedUserId);
                return Ok(new { message = "Sikeres likeolás." });
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [Authorize]
        [HttpPost("dislike")]
        public async Task<IActionResult> DislikeUser([FromBody] RequestDTO request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            if (request == null || string.IsNullOrEmpty(request.RequestedUserId))
            {
                return BadRequest("Érvénytelen kérés: hiányzó felhasználó ID.");
            }
            try
            {
                await _matchService.DislikeUser(userId, request.RequestedUserId);
                return Ok(new { message = "Sikeres dislikeolás." });
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

    }
}
