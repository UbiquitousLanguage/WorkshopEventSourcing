using System.Threading.Tasks;
using EventStore.ClientAPI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rating.Domain;
using Rating.Framework;
using Rating.Modules.Rating;
using Swashbuckle.AspNetCore.Swagger;
using static System.Environment;

namespace Rating
{
    public class Startup
    {
        private static readonly Serilog.ILogger Log = Serilog.Log.ForContext<Startup>();

        public Startup(IHostingEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }
        private IHostingEnvironment Environment { get; }

        public void ConfigureServices(IServiceCollection services)
            => ConfigureServicesAsync(services).GetAwaiter().GetResult();

        private async Task ConfigureServicesAsync(IServiceCollection services)
        {
            var gesConnection = EventStoreConnection.Create(
                Configuration["EventStore:ConnectionString"],
                ConnectionSettings.Create().KeepReconnecting(),
                Environment.ApplicationName);

            gesConnection.Connected += (sender, args)
                => Log.Information("Connection to {endpoint} event store established.", args.RemoteEndPoint);

            await gesConnection.ConnectAsync();

            var serializer = new JsonNetSerializer();

            var typeMapper = new TypeMapper()
                .Map<Events.V1.DealRateAddedToUserRating>("DealRateAddedToUserRating");

            var aggregateStore = new GesAggregateStore(
                (type, id) => $"{type.Name}-{id}",
                gesConnection,
                serializer,
                typeMapper);

            services.AddSingleton(new UserRatingApplicationService(aggregateStore));

            services.AddMvc();
            services.AddSwaggerGen(c =>
            {
                c.IncludeXmlComments($"{CurrentDirectory}/Rating.xml");
                c.SwaggerDoc(
                    $"v{Configuration["Swagger:Version"]}", 
                    new Info {
                        Title   = Configuration["Swagger:Title"], 
                        Version = $"v{Configuration["Swagger:Version"]}"
                    });
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMvc();
            app.UseMvcWithDefaultRoute();
            app.UseSwagger();
            app.UseSwaggerUI(options => options.SwaggerEndpoint(
                Configuration["Swagger:Endpoint:Url"], 
                Configuration["Swagger:Endpoint:Name"]));
        }
    }
}
