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
    public class LikesController : ControllerBase
    {
        private readonly MatchService _matchService;
        private readonly DiscoveryService _discoveryService;

        public LikesController(MatchService matchService, DiscoveryService discoveryService)
        {
            _matchService = matchService;
            _discoveryService = discoveryService;
        }

        [Authorize]
        [HttpGet("likedYou")]
        public async Task<ActionResult<List<ProfileDTO>>> GetLikedYouForUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            try
            {                
                var likedYouUsers = await _discoveryService.GetLikedYouForUser(userId);
                var likedYous = likedYouUsers.Select(u => u.ToProfileDTO()).ToList();
                return Ok(likedYous);
            }
            catch (UnauthorizedAccessException e)
            {
                return Unauthorized(new { message = e.Message });
            }         
            
        }

        [Authorize]
        [HttpGet("likes")]
        public async Task<ActionResult<List<ProfileDTO>>> GetLikesForUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            try
            {                
                var likedYouUsers = await _discoveryService.GetLikesForUser(userId);
                var likedYous = likedYouUsers.Select(u => u.ToProfileDTO()).ToList();
                return Ok(likedYous);
            }
            catch (UnauthorizedAccessException e)
            {
                return Unauthorized(new { message = e.Message });
            }
        }

        [Authorize]
        [HttpGet("dislikes")]
        public async Task<ActionResult<List<ProfileDTO>>> GetDislikesForUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            try
            {
                var dislikedYouUsers = await _discoveryService.GetDislikesForUser(userId);
                var dislikedYous = dislikedYouUsers.Select(u => u.ToProfileDTO()).ToList();
                return Ok(dislikedYous);
            }
            catch (UnauthorizedAccessException e)
            {
                return Unauthorized(new { message = e.Message });
            }
        }

        [Authorize]
        [HttpPost("acceptLike")]
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
        [HttpPost("declineLike")]
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

        [Authorize]
        [HttpPost("undoLike")]
        public async Task<IActionResult> UndoLike([FromBody] RequestDTO request)
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
                await _matchService.UndoLike(userId, request.RequestedUserId);
                return Ok(new { message = "Sikeres like visszavonás." });
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [Authorize]
        [HttpPost("undoDislike")]
        public async Task<IActionResult> UndoDislike([FromBody] RequestDTO request)
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
                await _matchService.UndoDislike(userId, request.RequestedUserId);
                return Ok(new { message = "Sikeres dislike visszavonás." });
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

    }
}
