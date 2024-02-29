﻿using CleanArchitecture.Application.Interfaces.Services.Auth;
using CleanArchitecture.Application.Interfaces.Services.Auth.User;
using CleanArchitecture.Application.Interfaces.Services.Common;
using CleanArchitecture.Application.Interfaces.Services.Posts;
using CleanArchitecture.Infrastructure.Services.Auth;
using CleanArchitecture.Infrastructure.Services.Auth.User;
using CleanArchitecture.Infrastructure.Services.Common;
using CleanArchitecture.Infrastructure.Services.Posts;
using Contracts.Services;
using Infrastructure.Configurations;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Infrastructure.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddInfrastructureLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddServices();
            services.AddConfigurationSettings(configuration);
        }

        private static void AddServices(this IServiceCollection services)
        {
            services
                .AddTransient<IDateTimeService, DateTimeService>()
                .AddTransient<ISerializeService, SerializeService>()

                .AddScoped<ITokenService, TokenService>()
                .AddScoped<IClaimService, ClaimService>()

                .AddTransient<ICurrentUserService, CurrentUserService>()
                .AddTransient<IRoleService, RoleService>()
                .AddTransient<IUserService, UserService>()
                
                .AddTransient<IPostCategoryService, PostCategoryService>()
                .AddTransient<IBlogService, BlogService>()
                .AddTransient<IPostService, PostService>();
        }
        
        private static void AddConfigurationSettings(this IServiceCollection services, IConfiguration configuration)
        {
            var emailSetting = configuration.GetSection(nameof(SmtpEmailSetting)).Get<SmtpEmailSetting>();
            if (emailSetting == null) throw new ArgumentNullException("Smtp email setting is not configured");
            services.AddSingleton(emailSetting);
            
            var mediaSetting = configuration.GetSection(nameof(MediaSettings)).Get<MediaSettings>();
            if (mediaSetting == null) throw new ArgumentNullException("Media settings is not configured");
            services.AddSingleton(mediaSetting);
        }
    }
}