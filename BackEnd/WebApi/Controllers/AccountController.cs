using Backend.Domain.DTOs;
using Backend.Domain.Services;
using Backend.Infrastructure.DBEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Backend.WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AccountService _accountService;

        public AccountController(AccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO model)
        {
            var result = await _accountService.RegisterAsync(model);
            if (result.Succeeded) return Ok(new { message = "Sikeres regisztráció!" });
            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            var response = await _accountService.LoginAsync(model);
            if (response == null) return Unauthorized(new { message = "Hibás e-mail vagy jelszó." });
            return Ok(response);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> Refresh([FromBody] TokenRequestDTO model)
        {
            var response = await _accountService.RefreshTokenAsync(model);

            if (response == null)
                return Unauthorized(new { message = "Érvénytelen refresh token." });

            return Ok(response);
        }

    }
}
