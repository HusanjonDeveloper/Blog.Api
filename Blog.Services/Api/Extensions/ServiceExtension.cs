﻿using Blog.Common.Models.JwtOptions;
using Blog.Data.Context;
using Blog.Data.Repositories;
using Blog.Services.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Blog.Services.Api.Extensions;

public static class ServiceExtension
{
    public static void AddServices(this IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddDbContext<BlogDbContext>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IBlogRepository, BlogRepository>();
        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<UserService>();
        services.AddScoped<BlogService>();
        services.AddScoped<PostService>();
        services.AddScoped<JwtTokenService>();
        services.AddHttpContextAccessor();
        services.AddScoped<UserHelper>();


        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            var jwt = builder.Configuration.GetSection(nameof(JwtOption)).Get<JwtOption>();
            var signinKey = System.Text.Encoding.UTF32.GetBytes(jwt!.signinKey);

            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidIssuer = jwt!.Issuer,
                ValidAudience = jwt.Audience,
                ValidateIssuer = true,
                ValidateAudience = true,
                IssuerSigningKey = new SymmetricSecurityKey(signinKey),
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,

            };
        });

    }
}