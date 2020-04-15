using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SPADemoCRUD.Models;

namespace SPADemoCRUD
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    AppDataBaseContext context = services.GetRequiredService<AppDataBaseContext>();
                    IOptions<AppConfig> options = services.GetRequiredService<IOptions<AppConfig>>();
                    if (options.Value.HasDemoData)
                    {
                        SampleDemoData.Initialize(context);
                    }
                    if (options.Value.SetUserRootById > 0)
                    {
                        UserObjectModel user = context.Users.FirstOrDefault(x => x.Id == options.Value.SetUserRootById);
                        if (user != null)
                        {
                            user.Role = AccessLevelUserRolesEnum.ROOT;
                            context.SaveChanges();
                        }
                    }
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred seeding the DB.");
                }
            }
            host.Run();

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
