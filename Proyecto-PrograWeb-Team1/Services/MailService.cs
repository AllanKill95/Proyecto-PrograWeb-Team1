using MailKit.Net.Smtp;
using MimeKit;

public class MailService
{
    private readonly IConfiguration _configuration;

    public MailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmail(string to, string subject, string body)
    {
        var email = _configuration["EmailSettings:Email"];
        var appPassword = _configuration["EmailSettings:AppPassword"];

        var message = new MimeMessage();

        message.From.Add(
            MailboxAddress.Parse(email));

        message.To.Add(
            MailboxAddress.Parse(to));

        message.Subject = subject;

        message.Body = new TextPart("plain")
        {
            Text = body
        };

        using var smtp = new SmtpClient();

        await smtp.ConnectAsync(
            "smtp.gmail.com",
            587,
            MailKit.Security.SecureSocketOptions.StartTls);

        await smtp.AuthenticateAsync(
            email,
            appPassword);

        await smtp.SendAsync(message);

        await smtp.DisconnectAsync(true);
    }
}