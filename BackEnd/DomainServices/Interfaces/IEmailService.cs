namespace Backend.Domain.Interfaces
{
    public interface IEmailService
    {
        Task SendMatchNotificationAsync(string recipientEmail, string recipientName, string matchedUserName);
    }
}