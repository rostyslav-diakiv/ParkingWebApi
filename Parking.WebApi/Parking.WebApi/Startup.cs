using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Parking.WebApi
{
    using MS.Common.Utils;

    using Parking.BLL.Entities;
    using Parking.BLL.Interfaces;
    using Parking.WebApi.Extensions;

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
            services.ConfigureSwagger(Configuration);
            services.AddMvc();
            services.AddSingleton<IParkingEntity, ParkingEntity>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpStatusCodeExceptionMiddleware();

            app.UseConfiguredSwagger();

            app.UseMvc();
        }
    }
}
