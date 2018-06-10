﻿using System;
using System.Reflection;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Marketplace.Domain.ClassifiedAds;
using Marketplace.Framework;
using Marketplace.Projections;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;
using Swashbuckle.AspNetCore.Swagger;
using static System.Environment;

// ReSharper disable UnusedMember.Global

namespace Marketplace
{
    public class Startup
    {
        private static readonly Serilog.ILogger Log = Serilog.Log.ForContext<Startup>();
        
        public Startup(IHostingEnvironment environment, IConfiguration configuration)
        {
            Environment   = environment;
            Configuration = configuration;
        }

        private IConfiguration      Configuration { get; }
        private IHostingEnvironment Environment   { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        //public void ConfigureServices(IServiceCollection services)
        public void ConfigureServices(IServiceCollection services) 
            => ConfigureServicesAsync(services).GetAwaiter().GetResult();

        private async Task ConfigureServicesAsync(IServiceCollection services)
        {
            var gesConnection = EventStoreConnection.Create(
                Configuration["EventStore:ConnectionString"], 
                ConnectionSettings.Create().KeepReconnecting(),
                Environment.ApplicationName);
            
            gesConnection.Connected += (sender, args) 
                => Log.Information("Connection to {eventStore} event store established.", gesConnection.ConnectionName);
            
            await gesConnection.ConnectAsync();
 
            var serializer = new JsonNetSerializer();

            var typeMapper = new TypeMapper()
                .Map<Events.V1.ClassifiedAdCreated>("ClassifiedAdCreated")
                .Map<Events.V1.ClassifiedAdRenamed>("ClassifiedAdRenamed")
                .Map<Events.V1.ClassifiedAdPriceChanged>("ClassifiedAdPriceChanged")
                .Map<Events.V1.ClassifiedAdActivated>("ClassifiedAdActivated")
                .Map<Events.V1.ClassifiedAdDeactivated>("ClassifiedAdDeactivated")
                .Map<Events.V1.ClassifiedAdPublished>("ClassifiedAdPublished")
                .Map<Events.V1.ClassifiedAdMarkedAsSold>("ClassifiedAdMarkedAsSold");
            
            var aggregateStore = new GesAggregateStore(
                (type, id) => $"{type.Name}-{id}",
                gesConnection,
                serializer,
                typeMapper);
            
            services.AddSingleton(new ClassifiedAdsApplicationService(aggregateStore));
            
            var documentStore = ConfigureRaven(services);

            await ProjectionManagerBuilder.With
                .Connection(gesConnection)
                .Serializer(serializer)
                .TypeMapper(typeMapper)
                .CheckpointStore(new RavenCheckpointStore(() => documentStore.OpenAsyncSession()))
                .Projections(
                    new ClassifiedAdsByOwner(() => documentStore.OpenAsyncSession()),
                    new ActiveClassifiedAds(() => documentStore.OpenAsyncSession()))
                .Activate();
            
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

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseMvcWithDefaultRoute();
            app.UseSwagger();
            app.UseSwaggerUI(options => options.SwaggerEndpoint(
                Configuration["Swagger:Endpoint:Url"], 
                Configuration["Swagger:Endpoint:Name"]));
        }

        private IDocumentStore ConfigureRaven(IServiceCollection services)
        {
            var store = new DocumentStore {
                Urls     = new[] {Configuration["RavenDb:Url"]},
                Database = Configuration["RavenDb:Database"]
            };

            if (Environment.IsDevelopment()) store.OnBeforeQuery += (_, args) 
                => args.QueryCustomization.WaitForNonStaleResults();
            
            try 
            {
                store.Initialize();          
                Log.Information("Initialized {database} document store.", store.Database);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(
                    $"Failed to initialize \"{store.Database}\" document store!" +
                    $"Please check if https is properly configured in order to use the certificate.", ex);
            }

            try
            {
                var record = store.Maintenance.Server.Send(new GetDatabaseRecordOperation(store.Database));
                if (record == null) 
                {
                    store.Maintenance.Server
                        .Send(new CreateDatabaseOperation(new DatabaseRecord(store.Database)));

                    Log.Information("{database} document store created.", store.Database);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to ensure that \"{store.Database}\" document store exists!", ex);
            }
            
            try
            {
                IndexCreation.CreateIndexes(Assembly.GetExecutingAssembly(), store);
                Log.Information("{database} document store indexes created or updated.", store.Database);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to create \"{store.Database}\" document store indexes!", ex);
            }

            return store;
        }
    }
}
