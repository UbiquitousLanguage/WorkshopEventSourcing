using System.Threading.Tasks;
using Marketplace.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using static System.Environment;

namespace Marketplace
{
    public class Startup
    {
        static readonly Serilog.ILogger Log = Serilog.Log.ForContext<Startup>();

        public Startup(IHostingEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        IConfiguration Configuration { get; }
        IHostingEnvironment Environment { get; }

        public void ConfigureServices(IServiceCollection services)
            => ConfigureServicesAsync(services).GetAwaiter().GetResult();

        async Task ConfigureServicesAsync(IServiceCollection services)
        {
            services.AddMvc();

            services.AddSwaggerGen(c =>
            {
                c.IncludeXmlComments($"{CurrentDirectory}/Marketplace.xml");
                c.SwaggerDoc(
                    $"v{Configuration["Swagger:Version"]}",
                    new Info {
                        Title   = Configuration["Swagger:Title"],
                        Version = $"v{Configuration["Swagger:Version"]}"
                    });
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment environment)
        {
            app.UseExceptionMiddleware();
            app.UseMvcWithDefaultRoute();
            app.UseSwagger();
            app.UseSwaggerUI(options => options.SwaggerEndpoint(
                Configuration["Swagger:Endpoint:Url"],
                Configuration["Swagger:Endpoint:Name"]));
        }
    }
}
