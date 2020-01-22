using Contracts;
using MailKit.Net.Pop3;
using Microsoft.Extensions.Configuration;
using NETCore.MailKit.Core;
using NETCore.MailKit.Infrastructure.Internal;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reader
{
    public class EmailReader : IReader
    {
        private static readonly int MaxMails = 5000;
        private readonly MailKitOptions _mailOptions;
        private readonly IConfiguration _configuration;

        public EmailReader(MailKitOptions mailOptions, IConfiguration configuration, IEmailService emailService)
        {
            _configuration = configuration;
            _mailOptions = mailOptions;
        }

        public async Task<IReadOnlyCollection<EmailMessage>> Get()
        {
            var userSections = _configuration.GetSection("Tracked").GetSection("Users");
            var trackedUsers = userSections.AsEnumerable().Select(x => x.Value).Where(x => x != null);

            using (var emailClient = new Pop3Client())
            {
                emailClient.Connect(_mailOptions.Server);
                emailClient.AuthenticationMechanisms.Remove("XOAUTH2");
                emailClient.Authenticate(_mailOptions.Account, _mailOptions.Password);

                List<EmailMessage> emails = new List<EmailMessage>();
                for (int index = 0; index < emailClient.Count && index < MaxMails; index++)
                {
                    var message = await emailClient.GetMessageAsync(index);
                    if (message.From.Mailboxes.Any(x => trackedUsers.Contains(x.Address)) &&
                        message.Subject.Contains("Отпуск"))
                    {
                        var body = (MimeKit.TextPart)message.Body;
                        var from = message.From.Mailboxes.FirstOrDefault();
                        var emailMessage = new EmailMessage
                        {
                            From = $"{from?.Name}",
                            Subject = message.Subject,
                            Content = body?.Text ?? (!string.IsNullOrEmpty(message.HtmlBody) ? message.HtmlBody : message.TextBody),
                        };
                        emails.Add(emailMessage);
                    }
                }

                return emails;
            }
        }
    }
}
