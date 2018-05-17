using System;
using System.Linq;
using Marketplace.Domain.ClassifiedAds;
using Marketplace.Framework;
using Marketplace.Projections;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;
using Swashbuckle.AspNetCore.Swagger;

namespace Marketplace
{
    using System.Threading.Tasks;

    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        //public void ConfigureServices(IServiceCollection services)

        public void ConfigureServices(IServiceCollection services) => ConfigureServicesAsync(services).GetAwaiter().GetResult();

        private async Task ConfigureServicesAsync(IServiceCollection services)
        {
            var esConnection = await Defaults.GetConnection();
            var typeMapper = ConfigureTypeMapper();
            var serializer = new JsonNetSerializer();
            
            services.AddMvc();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info {Title = "Event Log API", Version = "v1"});
                c.IncludeXmlComments($"{AppDomain.CurrentDomain.BaseDirectory}/Marketplace.xml");
            });
            services.AddSingleton<IAggregateStore>(new GesAggregateStore(
                (type, id) => $"{type.Name}-{id}",
                esConnection,
                serializer,
                typeMapper
            ));
            services.AddScoped<ClassifiedAdsApplicationService>();

            var getSession = ConfigureRavenDb();

            await ProjectionManagerBuilder.With
                .Connection(esConnection)
                .CheckpointStore(new RavenCheckpointStore(getSession))
                .Serializer(serializer)
                .TypeMapper(typeMapper)
                .Projections(
                    new ClassifiedAdsByOwner(getSession),
                    new ActiveClassifiedAds(getSession)
                )
                .Activate();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMvcWithDefaultRoute();

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Event Log API V1"); });
        }

        private static TypeMapper ConfigureTypeMapper()
        {
            var mapper = new TypeMapper();
            
            mapper.Map<Events.V1.ClassifiedAdCreated>("ClassifiedAdCreated");
            mapper.Map<Events.V1.ClassifiedAdRenamed>("ClassifiedAdRenamed");
            mapper.Map<Events.V1.ClassifiedAdPriceChanged>("ClassifiedAdPriceChanged");
            mapper.Map<Events.V1.ClassifiedAdActivated>("ClassifiedAdActivated");
            mapper.Map<Events.V1.ClassifiedAdDeactivated>("ClassifiedAdDeactivated");
            mapper.Map<Events.V1.ClassifiedAdPublished>("ClassifiedAdPublished");
            mapper.Map<Events.V1.ClassifiedAdMarkedAsSold>("ClassifiedAdMarkedAsSold");

            return mapper;
        }

        private static Func<IAsyncDocumentSession> ConfigureRavenDb()
        {
            const string databaseName = "ClassifiedAds";

            var store = new DocumentStore
            {
                Urls = new[] {"http://localhost:8080"},
                Database = databaseName
            }.Initialize();
            
            var databaseNames = store.Maintenance.Server.Send(new GetDatabaseNamesOperation(0, 25));
            if (!databaseNames.Contains(databaseName))
                store.Maintenance.Server.Send(new CreateDatabaseOperation(new DatabaseRecord(databaseName)));
            
            return () => store.OpenAsyncSession();
        }
    }
}
