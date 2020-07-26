using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using EasyTwoJuetengBackend.DataContexts;
using EasyTwoJuetengBackend.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace EasyTwoJuetengBackend
{
    public class Startup
    {
#if (!DEBUG)
        private readonly string _easyTwoDatabaseConnectionString = "ProductionEasyTwo";
#else

        private readonly string _easyTwoDatabaseConnectionString = "DevelopmentEasyTwo";
#endif
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            StarterHelper.CreateAccountsIfNeccesary(Configuration.GetConnectionString(_easyTwoDatabaseConnectionString));
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Startup));
            services.AutoRegisterByInterFaceName("ICrudPattern");
            services.AutoRegisterByInterFaceName("IValidator");
            services.AutoRegisterByNameSpace("Persistence");
            services.AddHttpsContextAccessorService();
            services.AddDbContext<EasyTwoJuetengContext>(options => options.UseLazyLoadingProxies()
       .UseSqlServer(
      Configuration.GetConnectionString(_easyTwoDatabaseConnectionString),
      sqlServerOptions => sqlServerOptions.CommandTimeout(999)
      ));
            var issuer = Configuration.GetSection("TokenAuthentication:Issuer").Value;
            var audience = Configuration.GetSection("TokenAuthentication:Audience").Value;
            var tokenKey = Configuration["AppSettings:Token"];
            services.AddAuthenticationMethods(issuer, audience, tokenKey);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "Easy Two Jueteng REST API",
                        Version = "v1"
                    });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.XML";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath, true);
            });
            services.ConfigureCors();
            services.AddMemoryCache();
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseSwagger(); 
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = string.Empty;
            });
            app.UseCors("AllowSpecificOrigin");
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
