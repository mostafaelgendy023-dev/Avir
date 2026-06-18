using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;

public class EmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void SendReminderEmail(string toEmail, DateTime reminderDate)
    {
        var senderEmail = _configuration["EmailSettings:SenderEmail"];
        var senderPassword = _configuration["EmailSettings:SenderPassword"];
        var smtpHost = _configuration["EmailSettings:SmtpHost"];
        var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("AVIR", senderEmail));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = "AVIR - Self-Examination Reminder";
        message.Body = new TextPart("html")
        {
            Text = $@"
                <h2>Hi! It's time for your self-examination 💗</h2>
                <p>Your recommended self-exam date is: <strong>{reminderDate:MMMM dd, yyyy}</strong></p>
                <p>Early detection saves lives. Take 5 minutes for yourself today.</p>
                <br/>
                <p>– The AVIR Team</p>"
        };

        using var client = new SmtpClient();
        client.Connect(smtpHost, smtpPort, false);
        client.Authenticate(senderEmail, senderPassword);
        client.Send(message);
        client.Disconnect(true);
    }
}