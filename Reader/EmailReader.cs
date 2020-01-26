using Contracts;
using Limilabs.Client.POP3;
using Limilabs.Mail;
using Microsoft.Extensions.Configuration;
using Reader.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reader
{
    public class EmailReader : IReader
    {
        private readonly MailOptions _mailOptions;
        private readonly IConfiguration _configuration;

        public EmailReader(MailOptions mailOptions, IConfiguration configuration)
        {
            _configuration = configuration;
            _mailOptions = mailOptions;
        }

        public async Task<IReadOnlyCollection<NotificationData>> Get()
        {
            var userSections = _configuration.GetSection("Tracked").GetSection("Users");
            var trackedUsers = userSections.AsEnumerable().Select(x => x.Value).Where(x => x != null);

            using (var emailClient = new Pop3())
            {
                emailClient.Connect(_mailOptions.Server);
                emailClient.UseBestLogin(_mailOptions.Account, _mailOptions.Password);

                var data = new List<NotificationData>();
                foreach (var uid in emailClient.GetAll())
                {
                    var email = emailClient.GetMessageByUID(uid);
                    var message = new MailBuilder().CreateFromEml(email);

                    var appointment = message.Appointments?.FirstOrDefault()?.Event;

                    // Отслеживаем конкретные email адреса + фильтрация по заголовку.
                    if (message.From.Any(x => (!trackedUsers.Any() || trackedUsers.Contains(x.Address)) && 
                        message.Subject.Contains("Отпуск")) &&
                        appointment != null)
                    {
                        var notificationData = new NotificationData
                        {
                            Subject = message.Subject,
                            Start = appointment.Start.Value,
                            End = appointment.End.Value
                        };
                        data.Add(notificationData);
                    }
                }

                return data;
            }
        }
    }
}
