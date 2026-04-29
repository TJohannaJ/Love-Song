using Backend.Domain.DTOs;
using Backend.Domain.Services;
using Microsoft.AspNetCore.SignalR;

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

        try
        {
            // 2. Üzenet mentése a Service-en keresztül (itt dől el, hogy van-e match)
            var savedMessage = await _chatService.SaveMessageAsync(request);

            // 3. Küldés a címzettnek (Receiver)
            // A SignalR alapból a 'sub' claim alapján azonosítja a felhasználókat.
            await Clients.User(request.ReceiverId)
                .SendAsync("ReceiveMessage", savedMessage);

            // 4. Visszajelzés a küldőnek is, hogy sikeres volt a mentés
            await Clients.Caller.SendAsync("MessageSent", savedMessage);
        }
        catch (UnauthorizedAccessException ex)
        {
            // Ha nincs match, hibaüzenetet küldünk vissza a hívónak
            await Clients.Caller.SendAsync("Error", ex.Message);
        }
    }

    public override async Task OnConnectedAsync()
    {
        // Opcionális: Jelezhetjük a barátoknak, hogy online lett
        await base.OnConnectedAsync();
    }

}