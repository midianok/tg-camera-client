using IpCameraClient.Db;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using IpCameraClient.Abstractions;
using IpCameraClient.Model;
using IpCameraClient.Repository;
using System.IO;

namespace IpCameraClient.WebFacade
{
    public class Startup
    {
        private const string DB_NAME = "CameraClient.db";

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
            var settings = Configuration.Get<Settings>();

            services.AddDbContext<SQLiteContext>(options =>
            {
                options.UseSqlite($"Filename={DB_NAME}");
                
            });

            var context = new SQLiteContext();
            context.Seed(x =>
            {
                x.Cameras.Add(new Camera
                {
                    Auth = settings.DefaultCameraAuth,
                    CameraUrl = settings.DefaultCameraUrl,
                    Model = settings.DefaultCameraModelName
                });
            });
            
            services
                .AddMvc()
                .AddJsonOptions(options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

            Bot.Init(settings.TelegramBotToken);
            Bot.Api.SetWebhookAsync(settings.HostUrl).Wait();

            services.AddScoped<DbContext, SQLiteContext>();
            services.AddScoped<IRepository<Camera>, EfRepository<Camera>>();
            services.AddScoped<IRepository<Record>, EfRepository<Record>>(); 
        }
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc();
        }
    }

    public class Settings
    {
        public string TelegramBotToken { get; set; }
        public string HostUrl { get; set; }
        public string DefaultCameraModelName { get; set; }
        public string DefaultCameraUrl { get; set; }
        public string DefaultCameraAuth { get; set; }
    }
}
