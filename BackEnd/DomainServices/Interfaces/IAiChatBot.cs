using Backend.Domain.DTOs;

namespace Backend.Domain.Interfaces;

public interface IAiChatBot
{
    Task<AiChatResponseDTO> AskAsync(List<ChatMessageDTO> history, CancellationToken ct = default);
}