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

            services
                .AddMvc()
                .AddJsonOptions(options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);


            Bot.Init(settings.TelegramBotToken);
            if (settings.Ngrok)
            {
                var ngrockUrl = Ngrok.GetTunnelUrl();
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
            BsonMapper.Global.Entity<Record>().Id(a => a.Id);
            
            //seed
            if (!File.Exists($"{Directory.GetCurrentDirectory()}\\CameraClient.db"))
            {
                new LiteDbRepository<Camera>()
                    .Add(new Camera
                    {
                        Auth = settings.DefaultCameraAuth,
                        CameraUrl = settings.DefaultCameraUrl,
                        Model = settings.DefaultCameraModelName
                    });
                var accessedTelegramUserNames = settings.TelegramUsersAccess
                    .Split(';')
                    .Select(z => new TelegramUser {TelegramUserName = z });
                
                new LiteDbRepository<TelegramUser>()
                    .AddRange(accessedTelegramUserNames);
            }

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
        public bool Ngrok { get; set; }
    }
}
