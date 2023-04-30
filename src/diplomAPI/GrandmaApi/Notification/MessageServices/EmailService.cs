using GrandmaApi.Models.Configs;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace GrandmaApi.Notification.MessageServices;

public class EmailService : IEmailMessageService
{
    private readonly IOptions<SmtpClientConfig> _smtpConfig;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<SmtpClientConfig> smtpConfig, ILogger<EmailService> logger)
    {
        _smtpConfig = smtpConfig;
        _logger = logger;
    }

    public async Task SendAsync(string email, string subject, string message)
    {
        var config = _smtpConfig.Value;
        var emailMessage = new MimeMessage();

        if(!MailboxAddress.TryParse(email, out _))
        {
            _logger.LogError("Invalid email address {0}", email);
            return;
        }

        emailMessage.From.Add(new MailboxAddress(config.Username, config.Address));
        emailMessage.To.Add(new MailboxAddress("", email));
        emailMessage.Subject = subject;
        emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
        {
            Text = message
        };
        using var client = new SmtpClient();
        try
        {
            await client.ConnectAsync(config.SmtpAddress, config.Port, config.UseSsl);
            if (client.Capabilities.HasFlag(SmtpCapabilities.Authentication))
            {
                await client.AuthenticateAsync(config.Username, config.Password);
            }

            await client.SendAsync(emailMessage);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return;
        }
        await client.DisconnectAsync(true);
    }
}