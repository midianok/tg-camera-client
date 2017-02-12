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
                .AddEnvironmentVariables(); ;
            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            

            services.AddDbContext<SQLiteContext>(options =>
            {
                options.UseSqlite($"Filename=CameraClient.db");
                
            });

            services.AddScoped<DbContext, SQLiteContext>();
            services.AddScoped<IRepository<Camera>, EfRepository<Camera>>();
            services.AddScoped<IRepository<Record>, EfRepository<Record>>(); 
            
            services
                .AddMvc()
                .AddJsonOptions( options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore );
        }
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc();
        }
    }
}
