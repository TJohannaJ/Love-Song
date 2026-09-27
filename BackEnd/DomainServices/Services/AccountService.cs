using Backend.Domain.DTOs;
using Backend.Domain.Entities;
using Backend.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Domain.Services
{
    public class AccountService
    {
        private readonly IUserRepository _userRepo;
        private readonly IConfiguration _configuration;

        public AccountService(IUserRepository userRepo, IConfiguration configuration)
        {
            _userRepo = userRepo;
            _configuration = configuration;
        }

        public async Task<IdentityResult> RegisterAsync(RegisterDTO model)
        {
            var user = new User
            {
                Email = model.Email,
                FullName = model.FullName,
                BirthDate = model.BirthDate,
                City = model.City,
                Gender = model.Gender,
                Picture = ImageToBinary("..\\..\\peach.png"),
                ShortIntro = "Szia! Még nem írtam rövid bemutatkozást.",
                LongIntro = "Szia! Még nem írtam hosszú bemutatkozást."
            };
            return await _userRepo.CreateAsync(user, model.Password, "token");
        }

        public async Task<object?> LoginAsync(LoginDTO model)
        {
            var user = await _userRepo.FindByEmailAsync(model.Email);
            if (user == null || !await _userRepo.CheckPasswordAsync(user, model.Password))
                return null;

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim("FullName", user.FullName)
            };

            var token = CreateToken(authClaims);
            var refreshToken = GenerateRefreshToken();

            // Különválasztott hívás: Csak a tokent mentjük az AspNetUserTokens táblába
            await _userRepo.SetRefreshTokenAsync(user, refreshToken, DateTime.Now.AddDays(7));

            return new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                refreshToken = refreshToken,
                expiration = token.ValidTo,
                user = new { email = user.Email, fullName = user.FullName }
            };
        }

        public async Task<object?> RefreshTokenAsync(TokenDTO tokenDTO)
        {
            if (string.IsNullOrEmpty(tokenDTO.RefreshToken))
                return null;

            // A repository az AspNetUserTokens-ben keres
            var user = await _userRepo.FindByRefreshTokenAsync(tokenDTO.RefreshToken);

            if (user == null)
                return null;

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email!)
            };

            var newAccessToken = CreateToken(authClaims);
            var newRefreshToken = GenerateRefreshToken();

            // Csak a tokenek frissítése az új adatokkal
            await _userRepo.SetRefreshTokenAsync(user, newRefreshToken, DateTime.Now.AddDays(7));

            return new
            {
                accessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
                refreshToken = newRefreshToken
            };
        }

        private JwtSecurityToken CreateToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]!));
            return new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddMinutes(15),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public async Task<object?> RefreshTokenAsync(TokenRequestDTO model)
        {
            // 1. Megkeressük a felhasználót a refresh token alapján
            var user = await _userRepo.FindByRefreshTokenAsync(model.RefreshToken);

            if (user == null) return null;

            // 2. Új Claims lista összeállítása (mint a Login-nál)
            var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

            // 3. Új tokenek generálása
            var newAccessToken = CreateToken(authClaims);
            var newRefreshToken = GenerateRefreshToken();

            // 4. Az új refresh token elmentése az adatbázisba
            await _userRepo.SetRefreshTokenAsync(user, newRefreshToken, DateTime.Now.AddDays(7));

            return new
            {
                accessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
                refreshToken = newRefreshToken
            };
        }

        private byte[] ImageToBinary(string path) => File.Exists(path) ? File.ReadAllBytes(path) : Array.Empty<byte>();
    }
}
