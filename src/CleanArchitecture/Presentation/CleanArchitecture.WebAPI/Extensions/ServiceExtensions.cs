﻿using System.Text;
using CleanArchitecture.WebAPI.Auth;
using CleanArchitecture.WebAPI.Filter;
using Infrastructure.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CleanArchitecture.WebAPI.Extensions;

public static class ServiceExtensions
{
    public static void AddWebApiLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddServices();
        services.AddControllers();
        services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.CustomOperationIds(description =>
                description.TryGetMethodInfo(out var methodInfo) ? methodInfo.Name : null);
            options.SwaggerDoc("AdminAPI", new OpenApiInfo
            {
                Title = "Admin API",
                Version = "v1",
                Description = "API for CleanArchitecture Admin project",
            });
            options.ParameterFilter<SwaggerNullableParameterFilter>();
        });
        services.AddAuthenticationAndAuthorization(configuration);
    }

    private static void AddServices(this IServiceCollection services)
    {
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>()
            .AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
    }

    public static void AddCorsPolicy(this IServiceCollection services, IConfiguration configuration,
        string corsPolicy)
    {
        var allowedOrigins = configuration["AllowedOrigins"];
        if (string.IsNullOrEmpty(allowedOrigins))
            throw new ArgumentNullException("AllowedOrigins is not configured");
        services.AddCors(o => o.AddPolicy(corsPolicy, builder =>
        {
            builder.AllowAnyMethod()
                .AllowAnyHeader()
                .WithOrigins(allowedOrigins)
                .AllowCredentials();
        }));
    }
    
    private static void AddAuthenticationAndAuthorization(this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtTokenSettingsSection = configuration.GetSection("JwtTokenSettings");
        
        services.Configure<JwtTokenSettings>(jwtTokenSettingsSection);
        
        services.AddAuthentication(o =>
        {
            o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(cfg =>
        {
            cfg.RequireHttpsMetadata = false;
            cfg.SaveToken = true;
            
            var jwtTokenSettings = jwtTokenSettingsSection.Get<JwtTokenSettings>();
            
            cfg.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = jwtTokenSettings.Issuer,
                ValidAudience = jwtTokenSettings.Issuer,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtTokenSettings.Key))
            };
        });
    }
}