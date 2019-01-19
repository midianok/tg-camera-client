using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using IpCameraClient.Core;
using IpCameraClient.Core.Infrastructure;
using MihaZupan;
using Serilog;
using Telegram.Bot;

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
            var settings = Configuration.Get<Settings>();
            
            services.AddOptions();
            services.Configure<Settings>(Configuration);
            services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
            services
                .AddMvc()
                .AddJsonOptions(options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
            
            services.AddScoped<IGetRecordService, GetRecordService>(_ => new GetRecordService(settings.CameraImageUrl, settings.CameraAuth));
            services.AddScoped<ITelegramService, TelegramService>();
            
            services.AddScoped<IList<string>>(_ => settings.TelegramUsersAccess.Split(";").ToList());
            services.AddScoped(_ =>
            {
                if (string.IsNullOrWhiteSpace(settings.Proxy.Host))
                {
                    return new TelegramBotClient(settings.TelegramBotToken);
                }
                
                var proxy = new HttpToSocks5Proxy(settings.Proxy.Host, settings.Proxy.Port, settings.Proxy.User, settings.Proxy.Password);
                return new TelegramBotClient(settings.TelegramBotToken, proxy);
            });
            
            Log.Logger.Information("initialization end");
        }
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
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
            
            var client = serviceProvider.GetService<TelegramBotClient>();
            
            var settings = Configuration.Get<Settings>();
            if (settings.Ngrok)
            {
                var ngrokUrl = Ngrok.GetTunnelUrl();
                Log.Logger.Information($"Ngrok url: {ngrokUrl}");
                client.SetWebhookAsync(ngrokUrl + "/TelegramBot/Message").Wait();
            }
            else
            {
                client.SetWebhookAsync(settings.WebHookUrl + "/TelegramBot/Message").Wait();
            }
        }
    }
}
