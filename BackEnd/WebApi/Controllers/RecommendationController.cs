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
        private readonly RecommendationService _recommendationService;

        public RecommendationController(RecommendationService recommendationService)
        {
            _recommendationService = recommendationService;
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
                var recommendedUsers = await _recommendationService.GetRecommendationsForUser(userId);
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
                await _recommendationService.LikeUser(userId, request.RequestedUserId);
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
                await _recommendationService.DislikeUser(userId, request.RequestedUserId);
                return Ok(new { message = "Sikeres dislikeolás." });
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

    }
}
