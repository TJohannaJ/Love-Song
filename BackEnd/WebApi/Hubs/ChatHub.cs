using Backend.Domain.DTOs;
using Backend.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

[Authorize]
public class ChatHub : Hub
{
    private readonly ChatService _chatService;

    public ChatHub(ChatService chatService)
    {
        _chatService = chatService;
    }

    public async Task SendMessage(SendMessageRequestDTO request)
    {
        var senderId = Context.UserIdentifier; 
        if (string.IsNullOrEmpty(senderId))
            throw new HubException("User identity not found.");
        System.Diagnostics.Debug.WriteLine($"SignalR SenderId: {senderId}");
        request.SenderId = senderId; 

        try
        {
            var savedMessage = await _chatService.SaveMessageAsync(request);

            await Clients.User(request.ReceiverId)
                .SendAsync("ReceiveMessage", savedMessage);

            await Clients.Caller.SendAsync("MessageSent", savedMessage);
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("Error", ex.Message);
        }
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

}