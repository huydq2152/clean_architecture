﻿using CleanArchitecture.Application.Interfaces.Repositories.Posts;
using CleanArchitecture.Domain.Entities.Identity;
using CleanArchitecture.Persistence.Common;
using CleanArchitecture.Persistence.Common.Repositories;
using CleanArchitecture.Persistence.Contexts;
using CleanArchitecture.Persistence.Repositories;
using Contracts.Common.Interfaces;
using Contracts.Common.Interfaces.Repositories;
using Infrastructure.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Persistence.Extensions;

public static class ServiceExtension
{
    public static void AddPersistenceLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRepositories();
        services.AddDbContext(configuration);
        services.AddIdentity();
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        services
            .AddScoped<ApplicationContextSeed>()
            .AddTransient(typeof(IUnitOfWork), typeof(UnitOfWork))
            .AddTransient(typeof(IRepositoryBase<,>), typeof(RepositoryBase<,>))
            .AddTransient<IPostCategoryRepository, PostCategoryRepository>();
    }

    private static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
    }

    private static void AddIdentity(this IServiceCollection services)
    {
        services.AddIdentity<AppUser, AppRole>(options => options.SignIn.RequireConfirmedAccount = false)
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.Configure<IdentityOptions>(options =>
        {
            // Password settings.
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
            options.Password.RequiredUniqueChars = 1;

            // Lockout settings.
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // User settings.
            options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            options.User.RequireUniqueEmail = false;
        });
    }

    private static void AddAuthenticationAndAuthorization(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<JwtTokenSettings>(configuration.GetSection("JwtTokenSettings"));
        services.AddScoped<SignInManager<AppUser>, SignInManager<AppUser>>();
        services.AddScoped<UserManager<AppUser>, UserManager<AppUser>>();
        services.AddScoped<RoleManager<AppRole>, RoleManager<AppRole>>();
    }
}