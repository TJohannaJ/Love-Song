using System.ComponentModel;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Backend.Domain.DTOs;
using Backend.Domain.Entities;
using Backend.Domain.Services;
using Microsoft.AspNetCore.Http;

namespace Backend.Infrastructure.ExternalApis.Tools;

public class SemanticRecommendationTool
{
    private readonly DiscoveryService _discoveryService;
    private readonly UserService _userService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public List<ProfileDTO> LastRecommendedProfiles { get; private set; } = new();

    public SemanticRecommendationTool(
        DiscoveryService discoveryService, 
        UserService userService, 
        IHttpContextAccessor httpContextAccessor)
    {
        _discoveryService = discoveryService;
        _userService = userService;
        _httpContextAccessor = httpContextAccessor;
    }

    [Description("Recommends matching user profiles based on bio similarity, music preferences, age range, gender, city, and required common songs.")]
    public async Task<string> RecommendProfilesByBioComparison(
        [Description("The exact number of profiles requested by the user (e.g., 1 for 'one profile', 5 for 'five profiles'). Default is 3.")] int count = 3,
        [Description("Minimum age constraint if specified by user.")] int? minAge = null,
        [Description("Maximum age constraint if specified by user.")] int? maxAge = null,
        [Description("Gender constraint: 'Male', 'Female', or empty/null for any.")] string? gender = null,
        [Description("City location constraint (e.g. 'Budapest').")] string? city = null,
        [Description("Set to true if the recommended users MUST have at least 1 common song/artist with the current user.")] bool requireCommonSong = false)
    {
        LastRecommendedProfiles.Clear();

        var currentUserId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(currentUserId))
        {
            return "Unable to identify the current logged-in user.";
        }

        var currentUser = await _userService.GetUser(currentUserId);
        if (currentUser == null) return "Current user profile not found.";

        var candidates = await _discoveryService.GetRecommendationsForUser(currentUserId);
        if (candidates == null || !candidates.Any())
        {
            return "No profile recommendations available right now based on your general settings.";
        }

        // 1. PONTOS SZŰRÉS A FELHASZNÁLÓI PARAMÉTEREK ALAPJÁN
        var filteredCandidates = candidates.Where(c =>
        {
            // Életkor szűrés
            if (minAge.HasValue && c.Age < minAge.Value) return false;
            if (maxAge.HasValue && c.Age > maxAge.Value) return false;

            // Nem szűrés (kis/nagybetű függetlenül)
            if (!string.IsNullOrWhiteSpace(gender))
            {
                var targetGender = gender.Trim().ToLower();
                var userGender = (c.Gender ?? "").Trim().ToLower();
                
                // Kezeljük a magyar/angol megfelelőket is
                if (targetGender.StartsWith("nő") || targetGender.StartsWith("fem"))
                {
                    if (!userGender.StartsWith("female") && !userGender.StartsWith("nő") && !userGender.StartsWith("woman")) return false;
                }
                else if (targetGender.StartsWith("férfi") || targetGender.StartsWith("male"))
                {
                    if (!userGender.StartsWith("male") && !userGender.StartsWith("férfi") && !userGender.StartsWith("man")) return false;
                }
            }

            // Város szűrés
            if (!string.IsNullOrWhiteSpace(city))
            {
                if (string.IsNullOrWhiteSpace(c.City) || !c.City.Contains(city, StringComparison.OrdinalIgnoreCase))
                    return false;
            }

            // Kötelező közös dal szűrés
            if (requireCommonSong)
            {
                var hasCommonSong = currentUser.CommonSongs != null && c.CommonSongs != null &&
                    currentUser.CommonSongs.Any(cs => c.CommonSongs.Any(s => 
                        (!string.IsNullOrEmpty(cs.SpotifyTrackId) && cs.SpotifyTrackId == s.SpotifyTrackId) ||
                        (!string.IsNullOrEmpty(cs.Artist) && string.Equals(cs.Artist, s.Artist, StringComparison.OrdinalIgnoreCase))));

                if (!hasCommonSong) return false;
            }

            return true;
        }).ToList();

        if (!filteredCandidates.Any())
        {
            return $"No matching profiles found with the requested criteria (Age: {minAge}-{maxAge}, Gender: {gender ?? "any"}, City: {city ?? "any"}, Must have common song: {requireCommonSong}).";
        }

        // 2. RANGSOROLÁS A BIO ÉS ZENEI HASONLÓSÁGI PONTSZÁM ALAPJÁN
        var requestedCount = Math.Max(1, count); // Legalább 1 elem

        var rankedCandidates = filteredCandidates
            .Select(candidate => new
            {
                User = candidate,
                Score = CalculateSimilarityScore(currentUser, candidate),
                MatchReason = GetMatchReason(currentUser, candidate)
            })
            .OrderByDescending(x => x.Score) // A leghasonlóbbak az élre
            .Take(requestedCount)           // PONTOSAN A KÉRT DARABSZÁMOT TARTJUK MEG
            .ToList();

        var topCandidates = rankedCandidates.Select(x => x.User).ToList();

        // Kézi mapper a DTO-ra
        LastRecommendedProfiles = topCandidates.Select(MapToProfileDto).ToList();

        // Személyre szabott szöveges válasz összeállítása
        var candidateList = rankedCandidates.Select((item, index) =>
        {
            var u = item.User;
            return $"{index + 1}. **{u.FullName} ({u.Age}, {u.City})**\n" +
                   $"   - **Match Reason**: {item.MatchReason}";
        });

        return $"Here are the top {topCandidates.Count} profile(s) matching your criteria:\n\n{string.Join("\n\n", candidateList)}";
    }

    // Kiszámítja a két profil közötti hasonlósági pontszámot
    private double CalculateSimilarityScore(User user1, User user2)
    {
        double score = 0;

        var bio1 = $"{user1.ShortIntro} {user1.LongIntro}";
        var bio2 = $"{user2.ShortIntro} {user2.LongIntro}";

        var words1 = ExtractMeaningfulWords(bio1);
        var words2 = ExtractMeaningfulWords(bio2);

        // A) Közös szavak száma a bio-ban (+2 pont / szó)
        var sharedWords = words1.Intersect(words2, StringComparer.OrdinalIgnoreCase).ToList();
        score += sharedWords.Count * 2.0;

        // B) Jaccard hasonlóság (Közös szavak / Összes egyedi szó aránya)
        var totalUniqueWords = words1.Union(words2, StringComparer.OrdinalIgnoreCase).Count();
        if (totalUniqueWords > 0)
        {
            score += ((double)sharedWords.Count / totalUniqueWords) * 5.0;
        }

        // C) Közös Spotify előadók/dalok (+3 pont / előadó)
        if (user1.CommonSongs != null && user2.CommonSongs != null)
        {
            var commonArtists = user1.CommonSongs
                .Select(s => s.Artist)
                .Intersect(user2.CommonSongs.Select(s => s.Artist), StringComparer.OrdinalIgnoreCase)
                .Where(a => !string.IsNullOrWhiteSpace(a))
                .Distinct()
                .Count();

            score += commonArtists * 3.0;
        }

        return score;
    }

    private string GetMatchReason(User user1, User user2)
    {
        var bio1 = $"{user1.ShortIntro} {user1.LongIntro}";
        var bio2 = $"{user2.ShortIntro} {user2.LongIntro}";

        if (user1.CommonSongs != null && user2.CommonSongs != null)
        {
            var commonArtists = user1.CommonSongs
                .Select(s => s.Artist)
                .Intersect(user2.CommonSongs.Select(s => s.Artist), StringComparer.OrdinalIgnoreCase)
                .Where(a => !string.IsNullOrWhiteSpace(a))
                .ToList();

            if (commonArtists.Any())
            {
                return $"You both listen to **{string.Join(", ", commonArtists.Take(2))}**!";
            }
        }

        var words1 = ExtractMeaningfulWords(bio1);
        var words2 = ExtractMeaningfulWords(bio2);

        var sharedWords = words1.Intersect(words2, StringComparer.OrdinalIgnoreCase).ToList();

        if (sharedWords.Any())
        {
            var formattedWords = sharedWords.Select(w => char.ToUpper(w[0]) + w.Substring(1).ToLower());
            return $"You both mentioned **{string.Join(", ", formattedWords.Take(3))}** in your bios!";
        }

        return "Shared location/age preferences and complementary profile bio.";
    }

    private static HashSet<string> ExtractMeaningfulWords(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return new HashSet<string>();

        var cleanText = Regex.Replace(text, @"[^\w\s]", " ");

        var stopWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "a", "az", "egy", "és", "vagy", "hogy", "is", "nem", "sem", "de", "mint", "ha", "van", "vannak",
            "valamint", "ahogy", "aqui", "mit", "amikor", "akinek", "aki", "akik", "mind", "már", "még",
            "i", "you", "he", "she", "it", "we", "they", "and", "or", "but", "the", "a", "an", "in", "on", "at",
            "to", "for", "with", "my", "your", "his", "her", "their", "is", "are", "am", "was", "were", "be"
        };

        var words = cleanText.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                             .Where(w => w.Length > 2 && !stopWords.Contains(w));

        return new HashSet<string>(words, StringComparer.OrdinalIgnoreCase);
    }

    private static ProfileDTO MapToProfileDto(User u)
    {
        return new ProfileDTO
        {
            Id = u.Id,
            FullName = u.FullName ?? string.Empty,
            Age = u.Age,
            Picture = u.PictureUrl ?? string.Empty,
            City = u.City ?? string.Empty,
            Gender = u.Gender ?? string.Empty,
            ShortIntro = u.ShortIntro ?? string.Empty,
            LongIntro = u.LongIntro ?? string.Empty,
            CommonSongs = u.CommonSongs?.Select(cs => new CommonSongDTO
            {
                SpotifyTrackId = cs.SpotifyTrackId ?? string.Empty,
                Title = cs.Title ?? string.Empty,
                Artist = cs.Artist ?? string.Empty,
                PreviewUrl = cs.PreviewUrl
            }).ToList() ?? new List<CommonSongDTO>()
        };
    }
}