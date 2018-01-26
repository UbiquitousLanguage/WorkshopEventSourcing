using Marketplace.Domain.ClassifiedAds;
using Marketplace.Framework;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace Marketplace
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSwaggerGen(c =>
                c.SwaggerDoc("v1", new Info {Title = "Event Log API", Version = "v1"}));
            services.AddSingleton<IAggregateStore>(new GesAggregateStore(
                (type, id) => $"{type.Name}-{id}",
                Defaults.GetConnection().Result,
                new JsonNetSerializer(),
                ConfigureMapper()
            ));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }

            app.UseMvcWithDefaultRoute();

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Event Log API V1"); });
        }

        private static TypeMapper ConfigureMapper()
        {
            var mapper = new TypeMapper();
            mapper.Map<Events.V1.ClassifiedAdCreated>("ClassifiedAdCreated");
            mapper.Map<Events.V1.ClassifiedAdRenamed>("ClassifiedAdRenamed");
            mapper.Map<Events.V1.ClassifiedAdPublished>("ClassifiedAdPublished");
            mapper.Map<Events.V1.ClassifiedAdMarkedAsSold>("ClassifiedAdMarkedAsSold");

            return mapper;
        }
    }
}
