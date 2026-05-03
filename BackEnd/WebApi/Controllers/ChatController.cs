using Backend.Domain.DTOs;
using Backend.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Backend.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly MatchService _matchService;

        public ChatController(MatchService matchService)
        {
            _matchService = matchService;
        }

        [HttpGet("matches")]
        public async Task<ActionResult<List<MatchDTO>>> GetMatches()
        {
            // A UserId-t a tokenből vesszük ki (ahogy a Hub-ban is)
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var matches = await _matchService.GetMatchesForUserAsync(userId);
            return Ok(matches);
        }
    }
}
