using Backend.Domain.DTOs;
using Backend.Domain.Entities;
using Backend.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Domain.Services
{
    public class SpotifyService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly ITokenRepository _tokenRepo;

        public SpotifyService(HttpClient httpClient, IConfiguration configuration, ITokenRepository tokenRepository)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _tokenRepo = tokenRepository;
        }

        public async Task<bool> ConnectSpotifyAsync(string userId, string code)
        {
            var tokens = await ExchangeCodeForTokens(code);
            if (tokens == null) return false;

            await _tokenRepo.StoreTokensAsync(
                userId,
                tokens.AccessToken,
                tokens.RefreshToken,
                tokens.ExpiresIn);

            return true;
        }

        public Task<string> GetSpotifyLoginUrlAsync(string userId)
        {
            var clientId = _configuration["Spotify:ClientId"];
            var redirectUri = _configuration["Spotify:RedirectUri"];
            var scope = "user-read-private user-read-email user-top-read";

            var url = $"https://accounts.spotify.com/authorize?client_id={clientId}&response_type=code&redirect_uri={Uri.EscapeDataString(redirectUri)}&scope={Uri.EscapeDataString(scope)}&state={userId}";

            return Task.FromResult(url);
        }

        private async Task<SpotifyTokenResponseDTO> ExchangeCodeForTokens(string code)
        {
            var clientId = _configuration["Spotify:ClientId"];
            var clientSecret = _configuration["Spotify:ClientSecret"];
            var redirectUri = _configuration["Spotify:RedirectUri"];

            // Ellenőrzés, hogy ne küldjünk üres kérést
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
            {
                Console.WriteLine("Hiba: Hiányzó Spotify ClientId vagy ClientSecret az appsettings.json-ben!");
                return null;
            }

            var requestBody = new Dictionary<string, string>
                {
                    { "grant_type", "authorization_code" },
                    { "code", code },
                    { "redirect_uri", redirectUri },
                    { "client_id", clientId },
                    { "client_secret", clientSecret }
                };

            try
            {
                var response = await _httpClient.PostAsync("https://accounts.spotify.com/api/token", new FormUrlEncodedContent(requestBody));
                var rawResponse = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Spotify hiba ({response.StatusCode}): {rawResponse}");
                    return null;
                }

                return System.Text.Json.JsonSerializer.Deserialize<SpotifyTokenResponseDTO>(rawResponse);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Váratlan hiba a Spotify kommunikáció során: {ex.Message}");
                return null;
            }
        }
    }
}