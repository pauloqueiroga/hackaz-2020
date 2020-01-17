using System.Linq;
using System.Text.Json.Serialization;
using HackathonAlert.API.Core.Infrastructure;
using HackathonAlert.API.Core.Services;
using HackathonAlert.API.Core.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NSwag.AspNetCore;

namespace HackathonAlert.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<SourceList>(Configuration);
            services.AddScoped(sp => sp.GetService<IOptionsSnapshot<SourceList>>().Value);

            var connectionString = Configuration.GetSection("ConnectionString").Value;
            // TODO: Change this later

            // Uncomment this and comment the next two lines to test locally
            // var contextFactory = new InMemoryApiContextFactory(connectionString);

            var contextFactory = new MySqlAlertApiContextFactory(connectionString);
            contextFactory.AlertContext().Database.Migrate();

            services.AddSingleton<IAlertContextFactory>(sp => contextFactory);
            services.AddScoped<IAlertService, AlertService>();

            services.AddControllers()
                .AddJsonOptions(options =>
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

            services.AddSwaggerDocument(config =>
                {
                    config.GenerateEnumMappingDescription = true;
                    config.DocumentProcessors.Add(new SwaggerDocProcesser());
                }
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseOpenApi(d => d.Path = "/api/swagger/v1/swagger.json");
            app.UseSwaggerUi3(d =>
            {
                d.EnableTryItOut = false;
                d.Path = "/api";
                d.SwaggerRoutes.Add(new SwaggerUi3Route("Alert", "/api/swagger/v1/swagger.json"));
            });
        }
    }
}
