using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace Logement.Services
{
    public class EmailService
    {
        public struct Attachment
        {
            public string FileName { get; set; }
            public Stream Content { get; set; }
        }

        private class MailSettings
        {
            public string Host { get; set; }
            public int Port { get; set; }
            public string Sender { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
        }

        private readonly MailSettings _settings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _settings = configuration.GetSection(nameof(MailSettings)).Get<MailSettings>();
            _logger = logger;
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string htmlBody, IEnumerable<Attachment> attachments = null)
        {
            var message = new MimeMessage();
            message.Sender = MailboxAddress.Parse(_settings.Sender);
            message.From.Add(MailboxAddress.Parse(_settings.Sender));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = htmlBody;

            if (attachments != null)
            {
                foreach (var attachment in attachments)
                {
                    string mimeType = MimeTypes.GetMimeType(attachment.FileName);
                    var types = mimeType.Split('/');
                    var contentType = new ContentType(types[0], types[1]);
                    bodyBuilder.Attachments.Add(attachment.FileName, attachment.Content, contentType);
                }
            }

            message.Body = bodyBuilder.ToMessageBody();

            using (var smtpClient = new SmtpClient())
            {
                try
                {
                    await smtpClient.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls);
                    await smtpClient.AuthenticateAsync(_settings.Username, _settings.Password);
                    smtpClient.Timeout = 120000;

                    await smtpClient.SendAsync(message);
                    await smtpClient.DisconnectAsync(true);

                    // Email sent successfully
                    return true;
                }
                catch (SmtpCommandException ex)
                {
                    // Handle specific exceptions that occur during the SMTP communication
                    Console.WriteLine($"SMTP command exception: {ex.Message}");
                }
                catch (SmtpProtocolException ex)
                {
                    // Handle exceptions that are specific to the SMTP protocol
                    Console.WriteLine($"SMTP protocol exception: {ex.Message}");
                }
                catch (Exception ex)
                {
                    // Handle any other general exceptions that may occur
                    Console.WriteLine($"Exception occurred while sending email: {ex.Message}");
                }

                // Email sending failed
                return false;
            }
        }
    }
}
