using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reader;
using Reader.Options;
using Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using System.Windows.Forms;

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
                    var storage = serviceProvider.GetService<IStorage>();

                    await storage.Load();

                    var data = await reader.Get();

                    await storage.Save(data.ToArray());
                    await storage.Backup();

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
            environment.ApplicationName = "Notification";

            services.AddSingleton(environment);
            services.AddTransient<IConfiguration>(collection => configuration);
            services.AddSingleton(configuration.GetSection("EmailConfiguration").Get<MailOptions>());
            services.AddSingleton(configuration.GetSection("FileConfiguration").Get<Storage.Options.FileOptions>());
            services.AddTransient<IReader, EmailReader>();
            services.AddTransient<IStorage, FileStorage>();

            return services;
        }

        private static void Show(IReadOnlyCollection<Contracts.NotificationData> data)
        {
            foreach (var item in data)
            {
                //NotifyIcon icon = new NotifyIcon();

                //icon.Icon = new Icon("./alarm.ico");
                //icon.Visible = true;
                //icon.BalloonTipIcon = ToolTipIcon.Warning;
                //icon.BalloonTipTitle = "Напоминание!";
                //icon.BalloonTipText = $"{item.Subject} с {item.Start.ToString("dd.MM.yyyy")} по {item.End.ToString("dd.MM.yyyy")}";
                //icon.ShowBalloonTip(2000);
            }
        }
    }
}
