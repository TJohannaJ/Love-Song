using Backend.Domain.DTOs;
using Backend.Domain.Entities;
using Backend.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Backend.Domain.Services
{
    public class SpotifyService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly ITokenRepository _tokenRepo;
        private readonly ISongRepository _songRepo;
        private readonly IUserRepository _userRepo;
        private readonly EssentiaService _essentiaService;

        private const string EssentiaApiUrl = "http://localhost:5000/analyze";

        public SpotifyService(
            HttpClient httpClient,
            IConfiguration configuration,
            ITokenRepository tokenRepository,
            ISongRepository songRepository,
            IUserRepository userRepo,
            EssentiaService essentiaService)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _tokenRepo = tokenRepository;
            _songRepo = songRepository;
            _userRepo = userRepo;
            _essentiaService = essentiaService;
        }

        public string GetLoginUrl(string userId)
        {
            var clientId = _configuration["Spotify:ClientId"];
            var redirectUri = Uri.EscapeDataString(_configuration["Spotify:RedirectUri"]);
            var scopes = Uri.EscapeDataString("user-top-read");

            // show_dialog=true felvétele a fiókváltási/újra-engedélyezési felület kikényszerítésére
            return $"https://accounts.spotify.com/authorize?response_type=code&client_id={clientId}&scope={scopes}&redirect_uri={redirectUri}&state={userId}&show_dialog=true";
        }

        public async Task<bool> ExchangeCodeForTokensAsync(string code, string userId)
        {
            var clientId = _configuration["Spotify:ClientId"];
            var clientSecret = _configuration["Spotify:ClientSecret"];
            var redirectUri = _configuration["Spotify:RedirectUri"];

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
                var raw = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode) return false;

                var dto = JsonSerializer.Deserialize<SpotifyTokenResponseDTO>(raw);
                if (dto == null || string.IsNullOrEmpty(dto.AccessToken)) return false;

                // 1. Tokenek mentése
                await _tokenRepo.StoreTokensAsync(userId, dto.AccessToken, dto.RefreshToken, dto.ExpiresIn);

                // 2. Bejelentkezési szinkronizálás indítása (letöltés + DB mentés)
                await SyncUserTopTracksAsync(userId);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exchanging code for tokens: {ex.Message}");
                return false;
            }
        }

        // Adatbázisban lévő mentett dalok beolvasása (Gyors nézet)
        public async Task<IEnumerable<Song>> GetSavedUserTopSongsAsync(string userId)
        {
            return await _songRepo.GetUserTopSongsAsync(userId);
        }

        // Spotify API-ból letöltés és elmentés az adatbázisba
        public async Task<bool> SyncUserTopTracksAsync(string userId, int limit = 10)
        {
            var accessToken = await _tokenRepo.GetAccessTokenAsync(userId);
            if (string.IsNullOrEmpty(accessToken)) return false;

            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.spotify.com/v1/me/top/tracks?limit={limit}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var refreshed = await RefreshAccessTokenAsync(userId);
                if (!refreshed) return false;

                accessToken = await _tokenRepo.GetAccessTokenAsync(userId);
                request = new HttpRequestMessage(HttpMethod.Get, $"https://api.spotify.com/v1/me/top/tracks?limit={limit}");
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                response = await _httpClient.SendAsync(request);
            }

            if (!response.IsSuccessStatusCode) return false;

            var raw = await response.Content.ReadAsStringAsync();

            try
            {
                using var doc = JsonDocument.Parse(raw);
                var root = doc.RootElement;
                if (!root.TryGetProperty("items", out var items)) return false;

                var songsWithRank = new List<(Song Song, int Rank)>();
                int currentRank = 1;
                Console.WriteLine($"[Spotify Sync] Beérkezett elemek száma: {items.GetArrayLength()}");

                foreach (var item in items.EnumerateArray())
                {
                    var trackId = item.GetProperty("id").GetString() ?? string.Empty;
                    var title = item.GetProperty("name").GetString() ?? string.Empty;
                    var previewUrl = item.TryGetProperty("preview_url", out var p) ? p.GetString() : null;

                    var artistsList = new List<string>();
                    if (item.GetProperty("artists").ValueKind == JsonValueKind.Array)
                    {
                        foreach (var a in item.GetProperty("artists").EnumerateArray())
                        {
                            var name = a.GetProperty("name").GetString();
                            if (!string.IsNullOrEmpty(name)) artistsList.Add(name);
                        }
                    }

                    var artist = string.Join(", ", artistsList);

                    // --- SONG ENTITY ELŐÁLLÍTÁSA DEEZER FALLBACK-KEL ÉS ESSENTIA ANALÍZISSEL ---
                    var domainSong = await CreateSongEntityAsync(trackId, title, artist, previewUrl);

                    songsWithRank.Add((domainSong, currentRank));
                    currentRank++;
                }

                // 1. Dalok és felhasználói top dal kapcsolatok elmentése
                await _songRepo.SaveUserTopSongsAsync(userId, songsWithRank);

                // 2. MusicVector kiszámítása és frissítése az UserRepository segítségével
                var validSongs = songsWithRank.Select(s => s.Song).Where(s => s.Valence > 0 || s.Energy > 0).ToList();
                if (validSongs.Any())
                {
                    var avgValence = validSongs.Average(s => s.Valence);
                    var avgEnergy = validSongs.Average(s => s.Energy);
                    var avgDanceability = validSongs.Average(s => s.Danceability);
                    var avgAcousticness = validSongs.Average(s => s.Acousticness);
                    var avgBpm = validSongs.Average(s => s.NormalizedBpm);

                    string musicVector = $"{avgValence:F4},{avgEnergy:F4},{avgDanceability:F4},{avgAcousticness:F4},{avgBpm:F4}";

                    // IUserRepository hívás a tiszta architektúra megtartása érdekében
                    await _userRepo.UpdateMusicVectorAsync(userId, musicVector);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Spotify sync error: {ex.Message}");
                return false;
            }
        }

        // FŐ METÓDUS: Előállítja a Song entitást Deezer fallback-kel és Essentia elemzéssel
        public async Task<Song> CreateSongEntityAsync(string spotifyTrackId, string title, string artist, string? spotifyPreviewUrl)
        {
            var song = new Song
            {
                SpotifyTrackId = spotifyTrackId,
                Title = title,
                Artist = artist,
                PreviewUrl = spotifyPreviewUrl
            };

            // 1. Ha a Spotify nem adott preview_url-t, megpróbáljuk lekérni a Deezer-ről
            if (string.IsNullOrEmpty(song.PreviewUrl))
            {
                song.PreviewUrl = await FetchDeezerPreviewUrlAsync(artist, title);
            }

            if (string.IsNullOrEmpty(song.PreviewUrl))
            {
                Console.WriteLine($"[Warning] Nincs Preview URL a dalhoz: {song.Artist} - {song.Title}");
            }
            // 2. Ha van működő Preview URL-ünk, elemezzük az Essentiával
            if (!string.IsNullOrEmpty(song.PreviewUrl))
            {
                var analysis = await FetchEssentiaAnalysisAsync(song.PreviewUrl);
                if (analysis != null)
                {
                    song.Valence = analysis.Valence;
                    song.Energy = analysis.Energy;
                    song.Danceability = analysis.Danceability;
                    song.Acousticness = analysis.Acousticness;
                    song.NormalizedBpm = analysis.NormalizedBpm;
                }
            }

            return song;
        }

        // Segédfüggvény 1: Deezer Preview URL lekérése (Free fallback)
        private async Task<string?> FetchDeezerPreviewUrlAsync(string artist, string title)
        {
            try
            {
                var primaryArtist = artist.Split(',')[0].Trim();
                var cleanTitle = title.Split('-')[0].Split('(')[0].Trim();

                // Tiszta összetett keresés (pl. "Daft Punk One More Time")
                string query = Uri.EscapeDataString($"{primaryArtist} {cleanTitle}");
                string url = $"https://api.deezer.com/search?q={query}";

                var response = await _httpClient.GetFromJsonAsync<JsonObject>(url);
                var data = response?["data"]?.AsArray();

                if (data != null && data.Count > 0)
                {
                    var preview = data[0]?["preview"]?.ToString();
                    if (!string.IsNullOrEmpty(preview)) return preview;
                }

                // Fallback csak a tisztított címre
                string fallbackQuery = Uri.EscapeDataString(cleanTitle);
                var fallbackResponse = await _httpClient.GetFromJsonAsync<JsonObject>($"https://api.deezer.com/search?q={fallbackQuery}");
                var fallbackData = fallbackResponse?["data"]?.AsArray();

                return fallbackData?.Count > 0 ? fallbackData[0]?["preview"]?.ToString() : null;
            }
            catch
            {
                return null;
            }
        }

        // Segédfüggvény 2: Essentia elemzés lekérése a FastAPI szolgáltatástól
        private async Task<EssentiaFeaturesDTO?> FetchEssentiaAnalysisAsync(string previewUrl)
        {
            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

                var response = await _httpClient.PostAsJsonAsync(
                    EssentiaApiUrl,
                    new { url = previewUrl },
                    cts.Token
                );

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<EssentiaFeaturesDTO>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Essentia Warning]: Nem sikerült az elemzés ({ex.Message}). A dal mentése folytatódik elemzés nélkül.");
            }

            return null;
        }

        private async Task<bool> RefreshAccessTokenAsync(string userId)
        {
            var refreshToken = await _tokenRepo.GetRefreshTokenAsync(userId);
            if (string.IsNullOrEmpty(refreshToken)) return false;

            var clientId = _configuration["Spotify:ClientId"];
            var clientSecret = _configuration["Spotify:ClientSecret"];

            var requestBody = new Dictionary<string, string>
            {
                { "grant_type", "refresh_token" },
                { "refresh_token", refreshToken },
                { "client_id", clientId },
                { "client_secret", clientSecret }
            };

            try
            {
                var response = await _httpClient.PostAsync("https://accounts.spotify.com/api/token", new FormUrlEncodedContent(requestBody));
                var raw = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode) return false;

                var dto = JsonSerializer.Deserialize<SpotifyTokenResponseDTO>(raw);
                if (dto == null || string.IsNullOrEmpty(dto.AccessToken)) return false;

                await _tokenRepo.StoreTokensAsync(userId, dto.AccessToken, dto.RefreshToken, dto.ExpiresIn);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error refreshing Spotify token: {ex.Message}");
                return false;
            }
        }
    }
}