using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NETCore.MailKit;
using NETCore.MailKit.Core;
using NETCore.MailKit.Infrastructure.Internal;
using Parser;
using Reader;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                var serviceCollection = InitializeServiceLocator();
                using (var serviceProvider = serviceCollection.BuildServiceProvider())
                {

                    var reader = serviceProvider.GetService<IReader>();
                    var parser = serviceProvider.GetService<IParser>();

                    var messages = await reader.Get();
                    var data = await parser.Parse(messages);
                    
                    Show(data);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"message:{ex.Message}\r\nstack:{ex.StackTrace}");
            }
            finally
            {
                Console.WriteLine($"Обработка завершена, нажмите enter");
                Console.Read();
            }
        }

        private static IServiceCollection InitializeServiceLocator()
        {
            var services = new ServiceCollection();

            var environment = new HostingEnvironment();
            var configuration = new ConfigurationBuilder()
                            .AddJsonFile("config.json", optional: true)
                            .Build();

            environment.EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            environment.ApplicationName = "Vacation notification";

            services.AddSingleton(environment);
            services.AddTransient<IConfiguration>(collection => configuration);
            services.AddSingleton(configuration.GetSection("EmailConfiguration").Get<MailKitOptions>());
            services.AddTransient<IMailKitProvider, MailKitProvider>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IReader, EmailReader>();
            services.AddTransient<IParser, EmailParser>();

            return services;
        }

        private static void Show(IReadOnlyCollection<Contracts.VacationData> data)
        {
            foreach (var item in data)
            {
                NotifyIcon icon = new NotifyIcon();

                icon.Icon = new Icon("./alarm.ico");
                icon.Visible = true;
                icon.BalloonTipIcon = ToolTipIcon.Warning;
                icon.BalloonTipTitle = "Напоминание!";
                icon.BalloonTipText = $"Отпуск у {item.From} с {item.Start.ToString("dd.MM.yyyy")} по {item.End.ToString("dd.MM.yyyy")}";
                icon.ShowBalloonTip(2000);
            }
        }
    }
}
