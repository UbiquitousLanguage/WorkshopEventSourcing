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
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var esConnection = Defaults.GetConnection().Result;
            var typeMapper = ConfigureTypeMapper();

            services.AddMvc();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info {Title = "Event Log API", Version = "v1"});
                c.IncludeXmlComments($"{AppDomain.CurrentDomain.BaseDirectory}/Marketplace.xml");
            });
            services.AddSingleton<IAggregateStore>(new GesAggregateStore(
                (type, id) => $"{type.Name}-{id}",
                esConnection,
                new JsonNetSerializer(),
                typeMapper
            ));

            var openSession = ConfgiureRavenDb();

            ProjectionManagerBuilder.With
                .Connection(esConnection)
                .CheckpointStore(new RavenCheckpointStore(openSession))
                .Serializer(new JsonNetSerializer())
                .TypeMapper(typeMapper)
                .Projections(
                    new MyClassifiedAdsProjection(openSession),
                    new ActiveAdsProjection(openSession)
                )
                .Activate().Wait();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseMvcWithDefaultRoute();

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Event Log API V1"); });
        }

        private static TypeMapper ConfigureTypeMapper()
        {
            var mapper = new TypeMapper();
            mapper.Map<Events.V1.ClassifiedAdCreated>("ClassifiedAdCreated");
            mapper.Map<Events.V1.ClassifiedAdRenamed>("ClassifiedAdRenamed");
            mapper.Map<Events.V1.ClassifiedAdPublished>("ClassifiedAdPublished");
            mapper.Map<Events.V1.ClassifiedAdMarkedAsSold>("ClassifiedAdMarkedAsSold");

            return mapper;
        }

        private static Func<IAsyncDocumentSession> ConfgiureRavenDb()
        {
            const string dbName = "ClassifiedAds";

            var store = new DocumentStore
            {
                Urls = new[] {"http://localhost:8080"},
                Database = dbName
            }.Initialize();
            
            var databaseNames = store.Maintenance.Server.Send(new GetDatabaseNamesOperation(0, 25));
            if (!databaseNames.Contains(dbName))
                store.Maintenance.Server.Send(new CreateDatabaseOperation(new DatabaseRecord(dbName)));
            
            return () => store.OpenAsyncSession();
        }
    }
}
