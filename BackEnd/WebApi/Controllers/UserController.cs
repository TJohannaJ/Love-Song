using Backend.Domain.DTOs;
using Backend.Domain.Services;
using Backend.WebApi.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Backend.Domain.Interfaces;

namespace BackEnd.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IBlobService _blobService;

        public UserController(UserService userService,  IBlobService blobService)
        {
            _userService = userService;
            _blobService = blobService;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UserDTO>> GetUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            try
            {                
                var profile = await _userService.GetUser(userId);
                if (profile == null) return NotFound();
                var profileDTO = profile.ToUserDTO();                
                return Ok(profileDTO);
            }
            catch (UnauthorizedAccessException e)
            {
                return Unauthorized(new { message = e.Message });
            }
        }

        [Authorize]
        [HttpPatch("updateProfile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UserDTO model)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            try
            {
                await _userService.UpdateProfileAsync(userId, model);
                return Ok(new { message = "Profil sikeresen frissítve!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [Authorize]
        [HttpPost("generateBio")]
        public async Task<ActionResult<GeneratedBioResultDTO>> GenerateBio([FromBody] GenerateBioDTO dto, CancellationToken ct)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var result = await _userService.GenerateBioWithAiAsync(userId, dto, ct);
                return Ok(result); // Visszaadja a { shortIntro, longIntro } párhuzamosan
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [Authorize]
        [HttpGet("get-upload-url")]
        public IActionResult GetUploadUrl([FromQuery] string fileName, [FromQuery] string contentType)
        {
            try
            {
                var result = _blobService.GenerateUploadSasUrl(fileName, contentType);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
