using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoorAccessManager.Api.Infrastructure.Attributes;
using DoorAccessManager.Api.Infrastructure.Middlewares;
using DoorAccessManager.Core.Services;
using DoorAccessManager.Core.Services.Abstract;
using DoorAccessManager.Data;
using DoorAccessManager.Items;
using DoorAccessManager.Items.Exceptions;
using System.Reflection;

namespace DoorAccessManager.Api
{
    public static class Extensions
    {
        public static IMvcBuilder AddControllersCustom(this IServiceCollection services)
        {
            return services.AddControllers(opt =>
            {
                opt.Filters.Add<ModelValidatorAttribute>();
            });
        }

        public static IApplicationBuilder UseErrorHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandlerMiddleware>();
        }

        public static IServiceCollection AddServices(this IServiceCollection services, Type implementationType)
        {
            services.AddTransient<IServiceBase, ServiceBase>();

            var allServices = implementationType.Assembly.GetTypes().Where(t => t.GetInterfaces().Any(i => i.IsAssignableFrom(typeof(IServiceBase))) && !t.IsInterface);

            foreach (var service in allServices)
            {
                var serviceType = service.GetInterfaces().Where(x => x.Name.Contains(service.Name)).FirstOrDefault() ??
                                     throw new BusinessException($"Base class did not found for type: {service.FullName}", 500);

                services.AddTransient(serviceType, service);
            }

            return services;
        }

        public static void AddValidators(this IMvcBuilder builder)
        {
            builder.Services.AddValidatorsFromAssemblyContaining<ItemIdentifier>();
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddFluentValidationClientsideAdapters();

            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = (context) =>
                {
                    var firstMessage = context.ModelState.Values.Where(v => v.Errors.Count > 0)
                                                                .SelectMany(v => v.Errors)
                                                                .Select(v => new
                                                                {
                                                                    Message = (!string.IsNullOrEmpty(v.ErrorMessage) || v.Exception is null) ? v.ErrorMessage : v.Exception.Message
                                                                })
                                                                .FirstOrDefault();

                    return new BadRequestObjectResult(new ErrorModel()
                    {
                        Message = firstMessage?.Message
                    });
                };
            });
        }

        public static void ConfigureDatabase(this IServiceCollection services, string? connectionString)
        {
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connectionString, x =>
            {
                x.MigrationsAssembly(typeof(DataIdentifier).Namespace);
                x.MigrationsHistoryTable("_Migrations");
            }));

            services.AddScoped(typeof(DbContext), typeof(ApplicationDbContext));
        }
    }
}
