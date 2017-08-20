using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using IpCameraClient.Model;
using System.IO;
using System.Linq;
using IpCameraClient.Infrastructure.Abstractions;
using IpCameraClient.Infrastructure.Repository;
using IpCameraClient.Infrastructure.Services;
using IpCameraClient.Model.Telegram;
using LiteDB;
using Serilog;

namespace IpCameraClient.WebFacade
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.RollingFile(Path.Combine(Directory.GetCurrentDirectory(), "Logs", "log-{Date}.txt"))
                .CreateLogger();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            Log.Logger.Information("Initialization start ");

            services.AddOptions();
            services.Configure<Settings>(Configuration);
            services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
            services
                .AddMvc()
                .AddJsonOptions(options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

            var settings = Configuration.Get<Settings>();
            if (!Directory.Exists(settings.ContentFolderName))
                Directory.CreateDirectory(settings.ContentFolderName);

            Bot.Init(settings.TelegramBotToken);
            if (settings.Ngrok)
            {
                var ngrockUrl = Ngrok.GetTunnelUrl();
                Log.Logger.Information($"Ngrok url: {ngrockUrl}");
                Bot.Api.SetWebhookAsync(ngrockUrl + "/TelegramBot/Message").Wait();
            }
            else
                Bot.Api.SetWebhookAsync(settings.HostUrl + "/TelegramBot/Message").Wait();

            services.AddScoped<IRepository<Camera>, LiteDbRepository<Camera>>();
            services.AddScoped<IRepository<Record>, LiteDbRepository<Record>>();
            services.AddScoped<IRepository<TelegramUser>, LiteDbRepository<TelegramUser>>();
            services.AddScoped<IRecordSaverService, FileSystemRecordSaverService>();
            
            BsonMapper.Global.Entity<Camera>().Id(a => a.Id);
            BsonMapper.Global.Entity<TelegramUser>().Id(a => a.Id);
            BsonMapper.Global.Entity<Record>().Id(a => a.Id).Ignore(x => x.Content);
            
            if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "CameraClient.db")))
            {
                Log.Logger.Information("Db not exists. Creating db.");
                new LiteDbRepository<Camera>()
                    .Add(new Camera
                    {
                        Auth = settings.DefaultCameraAuth,
                        CameraUrl = settings.DefaultCameraUrl,
                        Model = settings.DefaultCameraModelName
                    });

                var accessedTelegramUserNames = settings.TelegramUsersAccess
                    .Split(';')
                    .Select(userName => new TelegramUser { TelegramUserName = userName });
                new LiteDbRepository<TelegramUser>()
                    .AddRange(accessedTelegramUserNames);

                Log.Logger.Information("Db created");
            }
            
            Log.Logger.Information("initialization end");
        }
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                loggerFactory.AddConsole(LogLevel.Information);
                loggerFactory.AddDebug();
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
        public bool Ngrok { get; set; }
    }
}
