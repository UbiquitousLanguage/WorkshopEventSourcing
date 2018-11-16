﻿using System;
using System.Reflection;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Marketplace.Domain.ClassifiedAds;
using Marketplace.Framework;
using Marketplace.Infrastructure;
using Marketplace.Infrastructure.EventStore;
using Marketplace.Infrastructure.JsonNet;
using Marketplace.Infrastructure.Purgomalum;
using Marketplace.Infrastructure.RavenDB;
using Marketplace.Modules.ClassifiedAds;
using Marketplace.Modules.ClassifiedAds.Projections;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Session;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;
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
            var gesConnection = EventStoreConnection.Create(
                Configuration["EventStore:ConnectionString"],
                ConnectionSettings.Create().KeepReconnecting(),
                Environment.ApplicationName);

            gesConnection.Connected += (sender, args)
                => Log.Information("Connection to {endpoint} event store established.", args.RemoteEndPoint);

            await gesConnection.ConnectAsync();

            var serializer = new JsonNetSerializer();

            var typeMapper = new TypeMapper()
                .Map<Events.V1.ClassifiedAdRegistered>("Marketplace.V1.ClassifiedAdRegistered")
                .Map<Events.V1.ClassifiedAdTitleChanged>("Marketplace.V1.ClassifiedAdTitleChanged")
                .Map<Events.V1.ClassifiedAdTextChanged>("Marketplace.V1.ClassifiedAdTextUpdated")
                .Map<Events.V1.ClassifiedAdPriceChanged>("Marketplace.V1.ClassifiedAdPriceChanged")
                .Map<Events.V1.ClassifiedAdPublished>("Marketplace.V1.ClassifiedAdPublished")
                .Map<Events.V1.ClassifiedAdSold>("Marketplace.V1.ClassifiedAdMarkedAsSold")
                .Map<Events.V1.ClassifiedAdRemoved>("Marketplace.V1.ClassifiedAdRemoved");

            var aggregateStore = new GesAggregateStore(
                (type, id) => $"{type.Name}-{id}",
                gesConnection,
                serializer,
                typeMapper);

            var purgomalumClient = new PurgomalumClient();

            services.AddSingleton(new ClassifiedAdsApplicationService(
                aggregateStore, () => DateTimeOffset.UtcNow, text => purgomalumClient.CheckForProfanity(text)));

            var documentStore = ConfigureRaven();

            IAsyncDocumentSession GetSession() => documentStore.OpenAsyncSession();

            services.AddSingleton<Func<IAsyncDocumentSession>>(GetSession);

            services.AddSingleton(new ClassifiedAdsQueryService(GetSession));

            await ProjectionManager.With
                .Connection(gesConnection)
                .Serializer(serializer)
                .TypeMapper(typeMapper)
                .CheckpointStore(new RavenCheckpointStore(GetSession))
                .Projections(
                    new SoldClassifiedAdsProjection(GetSession),
                    new AvailableClassifiedAdsProjection(GetSession))
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

        public void Configure(IApplicationBuilder app, IHostingEnvironment environment)
        {
            app.UseExceptionMiddleware();
            app.UseMvcWithDefaultRoute();
            app.UseSwagger();
            app.UseSwaggerUI(options => options.SwaggerEndpoint(
                Configuration["Swagger:Endpoint:Url"],
                Configuration["Swagger:Endpoint:Name"]));
        }

        IDocumentStore ConfigureRaven()
        {
            var store = new DocumentStore {
                Urls     = new[] {Configuration["RavenDb:Url"]},
                Database = Configuration["RavenDb:Database"]
            };

            if (Environment.IsDevelopment()) store.OnBeforeQuery += (_, args)
                => args.QueryCustomization
                    .WaitForNonStaleResults()
                    .AfterQueryExecuted(result =>
                    {
                        Log.ForContext("SourceContext", "Raven").Debug("{index} took {duration}", result.IndexName, result.DurationInMs);
                    });

            try
            {
                store.Initialize();
                Log.Information("Connection to {url} document store established.", store.Urls[0]);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(
                    $"Failed to establish connection to \"{store.Urls[0]}\" document store!" +
                    $"Please check if https is properly configured in order to use the certificate.", ex);
            }

            try
            {
                var record = store.Maintenance.Server.Send(new GetDatabaseRecordOperation(store.Database));
                if (record == null)
                {
                    store.Maintenance.Server
                        .Send(new CreateDatabaseOperation(new DatabaseRecord(store.Database)));

                    Log.Debug("{database} document store database created.", store.Database);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(
                    $"Failed to ensure that \"{store.Database}\" document store database exists!", ex);
            }

            try
            {
                IndexCreation.CreateIndexes(Assembly.GetExecutingAssembly(), store);
                Log.Information("{database} document store database indexes created or updated.", store.Database);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to create or update \"{store.Database}\" document store database indexes!", ex);
            }

            return store;
        }
    }
}
