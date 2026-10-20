using Backend.Domain.DTOs;
using Backend.Domain.Interfaces;

namespace Backend.Domain.Services;

public class ChatBotService
{
    private readonly IAiChatBot _aiChatBot;

    public ChatBotService(IAiChatBot aiChatBot)
    {
        _aiChatBot = aiChatBot;
    }

    public async Task<AiChatResponseDTO> ProcessMessageAsync(List<ChatMessageDTO> history, CancellationToken ct = default)
    {
        return await _aiChatBot.AskAsync(history, ct);
    }
}