using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyTwoJuetengBackend.Helpers
{
    public static class ServiceExtension
    {
        public static void AutoRegisterByNameSpace(this IServiceCollection containerRegistry, string name)
        {
            var classes = GetRespositoryPatternClassesByNameSpace(name);
            foreach (var classs in classes)
                containerRegistry
                    .AddScoped
                    (classs
                    .GetInterfaces()
                    .FirstOrDefault
                    (x => x.Name.Contains($"I{classs.Name}"))
                    , classs);
        }
        public static void AddHttpsContextAccessorService(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
        }
        public static void AutoRegisterByInterFaceName(this IServiceCollection containerRegistry, string interfaceName)
        {
            var classes = GetClasseseOfAnInterface(interfaceName);
            foreach (var classs in classes)
                foreach (var iInterface in classs.GetInterfaces()
                    .Where(x => x.Name.Contains(interfaceName))
                    .ToList())
                    containerRegistry.AddScoped(iInterface, classs);
        }
        private static List<Type> GetRespositoryPatternClassesByNameSpace(string name)
        {
            var filteredTypes = new List<Type>();
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => p.IsClass).OrderBy(x => x.Name).ToList();

            foreach (var type in types)
            {
                var interfaces = type.GetInterfaces();
                var isRepositoryPatternVerified = (interfaces.FirstOrDefault(x => x.Name.Contains($"I{type.Name}"))) != null;
                if (isRepositoryPatternVerified)
                    filteredTypes.Add(type);
            }
            return filteredTypes.Where(x => x.Namespace.Contains(name)).ToList();
        }
        private static List<Type> GetClasseseOfAnInterface(string interfaceName)
        {
            return (AppDomain.CurrentDomain
                     .GetAssemblies()
                     .SelectMany(x => x.GetTypes())
                     .Where(x => x.IsClass)
                     .OrderBy(x => x.Name))
                     .Where(x => x.GetInterfaces().Any(xx => xx.Name.Contains(interfaceName)))
                     .ToList();

        }

        public static IServiceCollection ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });

            return services;
        }
        public static IServiceCollection AddAuthenticationMethods(this IServiceCollection services, string issuer, string audience, string tokenKey)
        {
            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddCookie(cfg => cfg.SlidingExpiration = true)
                .AddJwtBearer(cfg =>
                {

                    cfg.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
                        ValidIssuer = issuer,
                        ValidateAudience = true,
                        ValidAudience = audience,
                        ValidateLifetime = true,
                    };
                });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));

                options.AddPolicy("Agent", policy => policy.RequireRole("Agent"));

                options.AddPolicy("Both", policy => policy.RequireRole("Admin","Agent"));
            });
            return services;
        }

    }
}
