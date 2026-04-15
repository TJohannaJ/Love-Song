using Backend.Domain.DTOs;
using Backend.Domain.Entities;
using Backend.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Backend.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RecommendationController : Controller
    {
        private readonly RecommendationService recommendationService;

        public RecommendationController(RecommendationService recommendationService)
        {
            this.recommendationService = recommendationService;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<User>>> GetRecomenndationsForUser()
        {
            try
            {               
                var recommendations = await Task.Run(() => recommendationService.GetRecommendationsForUser());
                return Ok(recommendations);
            }
            catch (UnauthorizedAccessException e)
            {
                return Unauthorized(new { message = e.Message });
            }
        }

        [Authorize]
        [HttpPost("like")]
        public async Task<IActionResult> LikeUser([FromBody] LikeRequestDTO request)
        {
            if (request == null || string.IsNullOrEmpty(request.LikedUserId))
            {
                return BadRequest("Érvénytelen kérés: hiányzó felhasználó ID.");
            }
            try
            {
                await Task.Run(() => recommendationService.LikeUser(request.LikedUserId));
                return Ok(new { message = "Sikeres likeolás." });
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [Authorize]
        [HttpPost("dislike")]
        public async Task<IActionResult> DislikeUser([FromBody] DislikeRequestDTO request)
        {
            if (request == null || string.IsNullOrEmpty(request.DislikedUserId))
            {
                return BadRequest("Érvénytelen kérés: hiányzó felhasználó ID.");
            }
            try
            {
                await Task.Run(() => recommendationService.DislikeUser(request.DislikedUserId));
                return Ok(new { message = "Sikeres dislikeolás." });
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

    }
}
