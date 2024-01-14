﻿using System.Reflection;
using CleanArchitectureDemo.Application.Interfaces.Repositories.Posts;
using CleanArchitectureDemo.Application.Interfaces.Services.Posts;
using CleanArchitectureDemo.Infrastructure.Repositories;
using CleanArchitectureDemo.Infrastructure.Services.Posts;
using CleanArchitectureDemo.WebAPI.Filter;
using Microsoft.OpenApi.Models;
using Shared.SeedWork;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CleanArchitectureDemo.WebAPI.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddWebApiLayer(this IServiceCollection services)
    {
        services.AddControllers();
        services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.CustomOperationIds(description =>
                description.TryGetMethodInfo(out var methodInfo) ? methodInfo.Name : null);
            options.SwaggerDoc("Admin API", new OpenApiInfo
            {
                Title = "Admin API",
                Version = "v1",
                Description = "API for CleanArchitectureDemo Admin project",
            });
            options.ParameterFilter<SwaggerNullableParameterFilter>();
        });

        services.AddInfrastructureServices();

        return services;
    }

    private static void AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IPostCategoryRepository, PostCategoryRepository>()
            .AddScoped<IPostCategoryService, PostCategoryService>();
    }
}