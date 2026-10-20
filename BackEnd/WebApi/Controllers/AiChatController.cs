using Backend.Domain.DTOs;
using Backend.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class AiChatController : ControllerBase
{
    private readonly ChatBotService _chatBotService;

    public AiChatController(ChatBotService chatBotService)
    {
        _chatBotService = chatBotService;
    }

    [HttpPost("message")]
    public async Task<IActionResult> SendMessage([FromBody] ChatRequestDto request, CancellationToken ct)
    {
        var result = await _chatBotService.ProcessMessageAsync(request.History, ct);
        return Ok(result);
    }
}

public record ChatRequestDto(List<ChatMessageDTO> History);