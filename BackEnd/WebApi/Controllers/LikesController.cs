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
        private readonly LikesService _likesService;

        public LikesController(LikesService likesService)
        {
            _likesService = likesService;
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
                var likedYouUsers = await _likesService.GetLikedYouForUser(userId);
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
                var likedYouUsers = await _likesService.GetLikesForUser(userId);
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
                var dislikedYouUsers = await _likesService.GetDislikesForUser(userId);
                var dislikedYous = dislikedYouUsers.Select(u => u.ToProfileDTO()).ToList();
                return Ok(dislikedYous);
            }
            catch (UnauthorizedAccessException e)
            {
                return Unauthorized(new { message = e.Message });
            }
        }

    }
}
