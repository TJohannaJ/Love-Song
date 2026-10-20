using Backend.Domain.Interfaces;
using Resend;

namespace Backend.Infrastructure.ExternalApis
{
    public class EmailService : IEmailService
    {
        private readonly IResend _resend;

        public EmailService(IResend resend)
        {
            _resend = resend;
        }

        public async Task SendMatchNotificationAsync(string recipientEmail, string recipientName, string matchedUserName)
        {
            var message = new EmailMessage
            {
                From = "LoveSong@resend.dev",
                To = { recipientEmail },
                Subject = "You have a new match! 🎉",
                HtmlBody = $"<h2>Hi {recipientName}!</h2><p>You have a match with <strong>{matchedUserName}</strong>!</p>"
            };

            await _resend.EmailSendAsync(message);
        }
    }
}