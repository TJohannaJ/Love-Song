using Backend.Domain.DTOs;
using Backend.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Backend.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly MatchService _matchService;
        private readonly ChatService _chatService;

        public ChatController(MatchService matchService, ChatService chatService)
        {
            _matchService = matchService;
            _chatService = chatService;
        }

        [HttpGet("matches")]
        public async Task<ActionResult<List<MatchDTO>>> GetMatches()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var matchedUsers = await _matchService.GetMatchesForUserAsync(userId);
            var result = new List<MatchDTO>();

            foreach (var user in matchedUsers)
            {
                var lastMsg = await _chatService.GetLastMessageBetweenUsersAsync(userId, user.Id);

                string senderName = string.Empty;
                if (lastMsg != null)
                {
                    senderName = lastMsg.SenderId == user.Id ? user.FullName : "Te";
                }

                result.Add(new MatchDTO
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Age = user.Age,
                    Picture = user.PictureUrl,
                    LastMessage = lastMsg?.Content,
                    LastMessageTime = lastMsg?.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
                    LastMessageSenderName = senderName
                });
            }
            return Ok(result);
        }

        [HttpGet("conversation/{otherUserId}")]
        public async Task<ActionResult<List<MessageDTO>>> GetConversation(string otherUserId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var messages = await _chatService.GetConversationAsync(userId, otherUserId);
            return Ok(messages);
        }
    }
}
