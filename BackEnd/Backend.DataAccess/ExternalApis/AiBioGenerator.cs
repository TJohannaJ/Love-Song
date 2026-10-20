using Backend.Domain.DTOs;
using Backend.Domain.Entities;
using Backend.Domain.Interfaces;
using Microsoft.Extensions.AI;
using System.Text.Json;

namespace Backend.Infrastructure.ExternalApis
{
    public class AiBioGenerator : IAiBioGenerator
    {
        private readonly IChatClient _chatClient;

        public AiBioGenerator(IChatClient chatClient)
        {
            _chatClient = chatClient;
        }

        public async Task<GeneratedBioResultDTO> GenerateBothBiosAsync(User user, GenerateBioDTO dto,
            CancellationToken ct = default)
        {
            var topSongsText = user.TopSongs != null && user.TopSongs.Any()
                ? string.Join(", ", user.TopSongs.Take(3).Select(s => $"{s.Song?.Title} - {s.Song?.Artist}"))
                : "various genres";

            var extraInstruction = !string.IsNullOrWhiteSpace(dto.ExtraContext)
                ? $"CRITICAL MANDATORY REQUIREMENT: You MUST include or address this special detail in the output: '{dto.ExtraContext}'."
                : "";

            var prompt = $@"
You are a dating profile assistant.
Create two profile bios in English for:
- Name: {user.FullName}
- City: {user.City}
- Favorite Songs: {topSongsText}
- Keywords: {string.Join(", ", dto.Keywords)}
- Tone: {dto.Tone}
{extraInstruction}

Return ONLY a raw JSON object with no markdown block formatting.
JSON Format:
{{
  ""shortIntro"": ""Write short bio here"",
  ""longIntro"": ""Write long bio here""
}}

The ShortIntro should be max. 25 words long and,
make the intros sound human.
";

            var options = new ChatOptions
            {
                MaxOutputTokens = 2000, // Megemelve, hogy ne vágja el a választ félúton
                Temperature = 0.7f
            };

            ChatResponse response = await _chatClient.GetResponseAsync(prompt, options, cancellationToken: ct);
            var content = response.Text?.Trim() ?? string.Empty;

            // 1. Tisztítás: Kódblokkok eltávolítása (```json és ```)
            if (content.Contains("```"))
            {
                content = content.Replace("```json", "").Replace("```", "").Trim();
            }

            // 2. Tisztítás: Csak a legkülső { és } közötti JSON kinyerése
            int firstBrace = content.IndexOf('{');
            int lastBrace = content.LastIndexOf('}');
            if (firstBrace != -1 && lastBrace != -1 && lastBrace > firstBrace)
            {
                content = content.Substring(firstBrace, lastBrace - firstBrace + 1);
            }

            try
            {
                var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<GeneratedBioResultDTO>(content, jsonOptions);

                if (result != null && !string.IsNullOrWhiteSpace(result.ShortIntro))
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AiBioGenerator JSON Parsing Error]: {ex.Message}");
                Console.WriteLine($"[Raw Content]: {content}");
            }

            return new GeneratedBioResultDTO
            {
                ShortIntro = "Singer and songwriter with a passion for music, great coffee, and good vibes.",
                LongIntro =
                    "I spend my time singing, writing songs, and exploring new music. Looking for someone who shares a love for arts, deep conversations, and memorable moments."
            };
        }
    }
}
