using Canvas.API.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Canvas.API.Services;

public class EmailService
{
    private readonly EmailSettings _settings;

    public EmailService(IOptions<EmailSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task SendNewAssignmentEmailAsync(string toEmail, string studentName, string assignmentName, string courseName)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
        message.To.Add(new MailboxAddress(studentName, toEmail));
        message.Subject = $"New Assignment Posted: {assignmentName}";

        message.Body = new TextPart("plain")
        {
            Text = $"Hi {studentName},\n\nA new assignment \"{assignmentName}\" has been posted in {courseName}.\n\nLog in to Canvas to view it.\n\nCanvas LMS"
        };

        using var client = new SmtpClient();
        await client.ConnectAsync(_settings.SmtpHost, _settings.SmtpPort, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_settings.SenderEmail, _settings.AppPassword);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}