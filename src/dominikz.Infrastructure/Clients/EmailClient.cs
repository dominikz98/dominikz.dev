using System.Net;
using System.Net.Mail;
using dominikz.Domain.Options;
using Microsoft.Extensions.Options;

namespace dominikz.Infrastructure.Clients;

public class EmailClient
{
    private readonly IOptions<EmailOptions> _options;
    private SmtpClient _client;

    public EmailClient(IOptions<EmailOptions> options)
    {
        _options = options;
        _client = new(_options.Value.Host, _options.Value.Port)
        {
            Credentials = new NetworkCredential(_options.Value.Username, _options.Value.Password),
            EnableSsl = true
        };
    }

    public void Send(string subject, Exception ex)
    {
        var message = $"Exception: {ex.Message}\nStackTrace: {ex.StackTrace}";
        if (ex.InnerException != null)
            message += $"\n\nInner Exception: {ex.InnerException.Message}\nInner StackTrace: {ex.InnerException.StackTrace}";

        Send(subject, message);
    }

    public void Send(string subject, string message, Attachment[]? attachments = null)
    {
        // Create email message
        var mailMessage = new MailMessage
        {
            From = new MailAddress(_options.Value.Username),
            Subject = subject,
            Body = message
        };
        mailMessage.To.Add(new MailAddress(_options.Value.Username));
        if (attachments is { Length: > 0 })
            foreach (var attachment in attachments)
                mailMessage.Attachments.Add(attachment);

        // Send email
        _client.Send(mailMessage);
    }
}