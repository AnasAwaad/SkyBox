using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;
using Microsoft.Extensions.Options;
using SkyBox.API.Settings;
namespace SkyBox.API.Services;

public class EmailService(IOptions<MailSettings> mailsetting, ILogger<EmailService> logger) : IEmailSender
{
    private readonly MailSettings _mailsetting = mailsetting.Value;
    private readonly ILogger<EmailService> _logger = logger;

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var message = new MimeMessage
        {
            Sender = MailboxAddress.Parse(_mailsetting.Email),
            Subject = subject,

        };
        message.To.Add(MailboxAddress.Parse(email));

        var builder = new BodyBuilder
        {
            HtmlBody = htmlMessage
        };
        message.Body = builder.ToMessageBody();
        using var smtp = new SmtpClient();

        _logger.LogInformation("sending to {email}", email);

        smtp.Connect(_mailsetting.Host, _mailsetting.Port, SecureSocketOptions.StartTls);
        smtp.Authenticate(_mailsetting.Email, _mailsetting.Password);
        await smtp.SendAsync(message);
        smtp.Disconnect(true);
    }
}
