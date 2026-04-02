using Backend.Domain.DTOs;
using Backend.Infrastructure.DBEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Backend.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<DBUser> _userManager;
        private readonly IConfiguration _configuration;
        public AccountController(UserManager<DBUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration=configuration;
        }

        private static byte[] ImageToBinary(string imagePath)
        {
            FileStream fS = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
            byte[] b = new byte[fS.Length];
            fS.Read(b, 0, (int)fS.Length);
            fS.Close();
            return b;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new DBUser { UserName = model.Email, Email = model.Email, FullName = model.FullName, BirthDate = model.BirthDate, City = model.City, SpotifyToken= "token", Gender=model.Gender, Preference=model.Preferences, Picture = ImageToBinary("C:\\Users\\Tibor\\Documents\\Önlab\\onlab_project\\peach.png"), ShortIntro="Szia! Még nem írtam rövid bemutatkozást, de nemsokára fogok. :D", LongIntro= "Szia! Még nem írtam hosszú bemutatkozást, de nemsokára fogok. :D" };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return Ok(new { message = "Sikeres regisztráció!", userId = user.Id });
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Email, user.Email!),         
                    new Claim("FullName", user.FullName),
                    new Claim("BirthDate", user.BirthDate.ToString("yyyy-MM-dd"))
                };

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]!));

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(3), 
                    claims: authClaims, 
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo,
                    user = new
                    {
                        email = user.Email,
                        fullName = user.FullName,
                        birthDate = user.BirthDate
                    },
                    message = "Sikeres bejelentkezés!"
                });

            }         

            return Unauthorized(new { message = "Sikertelen bejelentkezés. Hibás e-mail vagy jelszó." });
        }
    }
}
