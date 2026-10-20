using System.Security.Claims;
using System.Text.RegularExpressions;
using Backend.Domain.DTOs;
using Backend.Domain.Interfaces;
using Backend.Infrastructure.ExternalApis.Tools;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.AI;

namespace Backend.Infrastructure.ExternalApis;

public class AiChatBot : IAiChatBot
{
    private readonly IChatClient _chatClient;
    private readonly AppNavigationTool _navigationTool;
    private readonly FAQTool _faqTool;
    private readonly SemanticRecommendationTool _recommendationTool;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AiChatBot(
        IChatClient chatClient, 
        AppNavigationTool navigationTool, 
        FAQTool faqTool, 
        SemanticRecommendationTool recommendationTool,
        IHttpContextAccessor httpContextAccessor)
    {
        _chatClient = chatClient;
        _navigationTool = navigationTool;
        _faqTool = faqTool;
        _recommendationTool = recommendationTool;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<AiChatResponseDTO> AskAsync(List<ChatMessageDTO> history, CancellationToken ct = default)
    {
        var currentUserId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        var lastUserMsg = history.LastOrDefault(m => (m.Role ?? "").ToLower() == "user")?.Content ?? "";

        // Dynamically extract requested count from text if present (e.g. "show me 2 profiles" -> 2)
        int requestedCount = ExtractCountFromMessage(lastUserMsg, defaultCount: 3);

        var options = new ChatOptions
        {
            MaxOutputTokens = 400,
            Tools = new[]
            {
                AIFunctionFactory.Create(_navigationTool.GetPageNavigationInfo),
                AIFunctionFactory.Create(_faqTool.GetAnswer),
                AIFunctionFactory.Create(_recommendationTool.RecommendProfilesByBioComparison) 
            }
        };

        var fullHistory = new List<Microsoft.Extensions.AI.ChatMessage>
        {
            new Microsoft.Extensions.AI.ChatMessage(ChatRole.System, 
                $"You are the helpful AI Assistant for the LoveSong dating app. " +
                $"The current logged-in user ID is '{currentUserId}'. " +
                $"CRITICAL INSTRUCTIONS:\n" +
                $"1. Whenever the user asks for profile recommendations, matches, or users, " +
                $"YOU MUST ALWAYS CALL THE 'RecommendProfilesByBioComparison' TOOL IMMEDIATELY.\n" +
                $"2. If the user asks for N profiles (e.g., 'show me 2 profiles', 'give me 5 recommendations'), set count = N when calling the tool.\n" +
                $"3. NEVER write fake user profiles (e.g., 'Maya, Austin TX') in your text response. Rely strictly on the tool output.\n" +
                $"4. If the user asks about app navigation, call 'GetPageNavigationInfo'.")
        };

        foreach (var msg in history.TakeLast(6))
        {
            var role = (msg.Role ?? "").ToLower() == "user" ? ChatRole.User : ChatRole.Assistant;
            fullHistory.Add(new Microsoft.Extensions.AI.ChatMessage(role, msg.Content ?? ""));
        }

        int maxRetries = 2;
        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                Microsoft.Extensions.AI.ChatResponse response = await _chatClient.GetResponseAsync(fullHistory, options, cancellationToken: ct);

                if (!string.IsNullOrWhiteSpace(response.Text))
                {
                    // If the tool was executed and returned profiles, return response
                    if (_recommendationTool.LastRecommendedProfiles.Any())
                    {
                        return new AiChatResponseDTO 
                        { 
                            Reply = response.Text,
                            Profiles = _recommendationTool.LastRecommendedProfiles
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AiChatBot WARNING]: Attempt {attempt} error: {ex.Message}");
            }
        }

        // FALLBACK: Ha az LLM nem indította el a tool-t ajánlás kérésekor, közvetlenül meghívjuk a dinamikusan kinyert darabszámmal!
        if (lastUserMsg.ToLower().Contains("recommend") || lastUserMsg.ToLower().Contains("profile") || lastUserMsg.ToLower().Contains("show") || lastUserMsg.ToLower().Contains("give"))
        {
            var reply = await _recommendationTool.RecommendProfilesByBioComparison(count: requestedCount);
            return new AiChatResponseDTO
            {
                Reply = reply,
                Profiles = _recommendationTool.LastRecommendedProfiles
            };
        }

        var faqAnswer = _faqTool.GetAnswer(lastUserMsg);
        return new AiChatResponseDTO 
        { 
            Reply = !string.IsNullOrWhiteSpace(faqAnswer) ? faqAnswer : "I am here to help! Ask me for profile recommendations or app navigation guidance.",
            Profiles = _recommendationTool.LastRecommendedProfiles
        };
    }

    // Segédfüggvény a darabszám kinyerésére a mondatból (pl. "2 profiles", "one profile", "10 matches")
    private static int ExtractCountFromMessage(string text, int defaultCount)
    {
        if (string.IsNullOrWhiteSpace(text)) return defaultCount;

        var lower = text.ToLower();

        // Számszerű szavak felismerése
        if (lower.Contains("one ") || lower.EndsWith("one")) return 1;
        if (lower.Contains("two ")) return 2;
        if (lower.Contains("three ")) return 3;
        if (lower.Contains("four ")) return 4;
        if (lower.Contains("five ")) return 5;

        // Számjegyek keresése a szövegben (pl. "2", "5", "10")
        var match = Regex.Match(text, @"\b(\d+)\b");
        if (match.Success && int.TryParse(match.Value, out int count))
        {
            return count;
        }

        return defaultCount;
    }
}