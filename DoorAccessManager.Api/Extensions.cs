using DoorAccessManager.Api.Infrastructure.Attributes;
using DoorAccessManager.Api.Infrastructure.Authentication;
using DoorAccessManager.Api.Infrastructure.Middlewares;
using DoorAccessManager.Core.Services;
using DoorAccessManager.Core.Services.Abstract;
using DoorAccessManager.Data;
using DoorAccessManager.Data.Repositories;
using DoorAccessManager.Data.Repositories.Abstract;
using DoorAccessManager.Items;
using DoorAccessManager.Items.Authentication;
using DoorAccessManager.Items.Enums;
using DoorAccessManager.Items.Exceptions;
using FluentValidation;
using FluentValidation.AspNetCore;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

namespace DoorAccessManager.Api
{
    public static class Extensions
    {
        public static IMvcBuilder AddControllersCustom(this IServiceCollection services)
        {
            var policy = new AuthorizationPolicyBuilder()
                             .RequireAuthenticatedUser()
                             .Build();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<IContextAccessor, ContextAccessor>();

            return services.AddControllers(opt =>
            {
                opt.Filters.Add<ModelValidatorAttribute>();
                opt.Filters.Add(new AuthorizeFilter(policy));
            }).AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
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

        public static IServiceCollection AddRepositories(this IServiceCollection services, Type implementationType)
        {
            services.AddTransient<IRepositoryBase, RepositoryBase>();

            var allRepositories = implementationType.Assembly.GetTypes().Where(t => t.GetInterfaces().Any(i => i.IsAssignableFrom(typeof(IRepositoryBase))) && !t.IsInterface);

            foreach (var repository in allRepositories)
            {
                var repositoryType = repository.GetInterfaces().Where(x => x.Name.Contains(repository.Name)).FirstOrDefault() ??
                                     throw new BusinessException($"Base class did not found for type: {repository.FullName}", 500);

                services.AddTransient(repositoryType, repository);
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

        public static IServiceCollection AddJwt(this IServiceCollection services, Action<AuthorizationOptions> authorizationOptions = null)
        {
            IConfiguration configuration;
            ILogger<JwtOptions> logger;

            using (var serviceProvider = services.BuildServiceProvider())
            {
                configuration = serviceProvider.GetService<IConfiguration>()!;
                logger = serviceProvider.GetService<ILogger<JwtOptions>>()!;
            }

            var section = configuration.GetSection("jwt");
            services.Configure<JwtOptions>(section);
            var options = section.Get<JwtOptions>();

            services
                .AddAuthentication(config =>
                {
                    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    config.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer("Bearer", config =>
                {
                    config.Audience = options!.ValidIssuer;
                    config.RequireHttpsMetadata = false;
                    config.ClaimsIssuer = options.ValidIssuer;
                    config.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SecretKey)),
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidIssuer = options.ValidIssuer,
                        ValidAudience = options.ValidIssuer,
                        ClockSkew = TimeSpan.FromDays(1)
                    };
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy => policy.RequireRole(RoleTypes.Admin.ToString()));
                options.AddPolicy("OfficeManager", policy => policy.RequireRole(RoleTypes.OfficeManager.ToString()));
                options.AddPolicy("Employee", policy => policy.RequireRole(RoleTypes.Employee.ToString()));
                options.AddPolicy("All", policy =>
                    policy.RequireRole(RoleTypes.Admin.ToString(), RoleTypes.Employee.ToString(), RoleTypes.OfficeManager.ToString()));
                options.AddPolicy("Admin_OfficeManager", policy =>
                    policy.RequireRole(RoleTypes.Admin.ToString(), RoleTypes.OfficeManager.ToString()));
            });

            return services;
        }

        public static IServiceCollection AddSwaggerCustom(this IServiceCollection services)
        {
            return services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Door Access Manager API", Version = "v1" });


                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Example of using JWT Token: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                },
                                Name = "Bearer",
                                Scheme = "oauth2",
                                In = ParameterLocation.Header
                            },
                            new string[] { }
                        }
                    });

            });
        }

        public static IApplicationBuilder UseSwaggerCustom(this IApplicationBuilder builder)
        {
            builder.UseSwagger();

            builder.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Door Access Manager API");
            });

            return builder;
        }

        public static IServiceCollection AddMappings(this IServiceCollection services)
        {
            var config = TypeAdapterConfig.GlobalSettings;
            config.Scan(Assembly.GetAssembly(typeof(ItemIdentifier))!);

            services.AddSingleton(config);
            services.AddScoped<IMapper, ServiceMapper>();

            return services;
        }
    }
}
