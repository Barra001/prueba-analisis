using Fabrica;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ObligatorioDa2.Api.Filters;
using Microsoft.AspNetCore.Mvc;

namespace ObligatorioDa2.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();

            FabricaServicios fabrica = new FabricaServicios(services, Configuration);
            fabrica.AgregarServicios();
            fabrica.AgregarContexto();

         

            services.AddApiVersioning(setup =>
            {
                setup.DefaultApiVersion = new ApiVersion(1, 0);
                setup.AssumeDefaultVersionWhenUnspecified = true;
                setup.ReportApiVersions = true;
            });
            services.AddCors(c =>
            {
                c.AddPolicy(c.DefaultPolicyName,
                    optio => optio.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            });
            

            services.AddSwaggerGen();
            
       
      
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
               
            }

           
            app.UseRouting();
             app.UseCors();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ObligatorioDa2 v1");
           
            });
        

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                
            });
        }
    }
}
