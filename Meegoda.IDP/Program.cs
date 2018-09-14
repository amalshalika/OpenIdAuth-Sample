using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Meegoda.IDP.Entities;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Meegoda.IDP
{
    public static class ToDoMigration
    {
        public static IWebHost Migrate(this IWebHost webhost)
        {
            using (var scope = webhost.Services.GetService<IServiceScopeFactory>().CreateScope())
            {
                using (var dbContext = scope.ServiceProvider.GetRequiredService<TodoUserContext>())
                {
                    dbContext.Database.Migrate();
                    dbContext.EnsureSeedDataForContext();
                }
            }
            return webhost;
        }
    }
    public class Program
    {
        

        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);
            host.Migrate();
            host.Run();
        }

        

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
