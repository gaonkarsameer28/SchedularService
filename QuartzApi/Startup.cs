using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using QuartzApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuartzApi
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
           DBSettings ObjDBSettings = new DBSettings()
            {
                ConnectionString = Configuration.GetSection("ConnectionString").Value,
                APIManagerCollectionName = Configuration.GetSection("APIManagerCollectionName").Value,
                DatabaseName = Configuration.GetSection("DatabaseName").Value,
                //HourlyStatisticsCollectionName = Configuration.GetSection("HourlyStatisticsCollectionName").Value,
                QRTZ_SchedularCollectionName = Configuration.GetSection("QRTZ_SchedularCollectionName").Value,
                //ThresholdManagerCollectionName = Configuration.GetSection("ThresholdManagerCollectionName").Value,
                //ThresholdManagementCollectionName = Configuration.GetSection("ThresholdManagementCollectionName").Value,
                //SPAAjaxCallsThreadSleepCollectionName = Configuration.GetSection("SPAAjaxCallsThreadSleepCollectionName").Value
            };

            services.AddSingleton<DBSettings>(ObjDBSettings);
            
            services.AddSingleton<QRTZ_SchedularService>();
            


            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "QuartzApi", Version = "v1" });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "QuartzApi v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
