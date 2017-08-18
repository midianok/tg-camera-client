using IpCameraClient.Db;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using IpCameraClient.Model;
using System.IO;
using System.Linq;
using IpCameraClient.Infrastructure.Abstractions;
using IpCameraClient.Infrastructure.Repository;
using IpCameraClient.Infrastructure.Services;
using IpCameraClient.Model.Telegram;
using Serilog;

namespace IpCameraClient.WebFacade
{
    public class Startup
    {
        private const string DbName = "CameraClient.db";

        public IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables(); ;
            Configuration = builder.Build();

        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.Configure<Settings>(Configuration);
            services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));

            var settings = Configuration.Get<Settings>();
            if (!Directory.Exists(settings.ContentFolderName))
                Directory.CreateDirectory(settings.ContentFolderName);

            services.AddDbContext<SQLiteContext>(options =>
            {
                options.UseSqlite($"Filename={DbName}");
                
            });
            var optionsBuilder = new DbContextOptionsBuilder<SQLiteContext>();
            var context = new SQLiteContext(optionsBuilder.Options);
            context.Seed(x =>
            {
                x.Cameras.Add(new Camera
                {
                    Auth = settings.DefaultCameraAuth,
                    CameraUrl = settings.DefaultCameraUrl,
                    Model = settings.DefaultCameraModelName
                });

                var accessedTelegramUserNames = settings.TelegramUsersAccess
                    .Split(';')
                    .Select(z => new TelegramUser {TelegramUserName = z });

                x.TelegramUser.AddRange(accessedTelegramUserNames);

            });
            
            services
                .AddMvc()
                .AddJsonOptions(options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

            Bot.Init(settings.TelegramBotToken);
            Bot.Api.SetWebhookAsync(settings.HostUrl + "/TelegramBot/Message").Wait();

            services.AddScoped<DbContext, SQLiteContext>();
            services.AddScoped<IRepository<Camera>, EfRepository<Camera>>();
            services.AddScoped<IRepository<Record>, EfRepository<Record>>();
            services.AddScoped<IRepository<TelegramUser>, EfRepository<TelegramUser>>();
            services.AddScoped<IRecordSaverService, FileSystemRecordSaverService>();
        }
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                loggerFactory.AddConsole(LogLevel.Information);
                loggerFactory.AddDebug();
            }
            else
            {
                Log.Logger = new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .WriteTo.RollingFile(Path.Combine(Directory.GetCurrentDirectory(), "logs", "log-{Date}.txt"))
                    .CreateLogger();
            }

            app.UseMvc(routes => 
            {
                routes.MapRoute(
                    name: "DefaultRoute",
                    template: "{controller}/{action}/{id?}");
            });
        }
    }

    public class Settings
    {
        public string TelegramBotToken { get; set; }
        public string TelegramUsersAccess { get; set; }
        public string HostUrl { get; set; }
        public string DefaultCameraModelName { get; set; }
        public string DefaultCameraUrl { get; set; }
        public string DefaultCameraAuth { get; set; }
        public string ContentFolderName { get; set; }
    }
}
