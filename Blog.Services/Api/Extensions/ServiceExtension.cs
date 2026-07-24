using System.Text;
using Blog.Common.Models.JwtOptions;
using Blog.Data.Context;
using Blog.Data.Repositories;
using Blog.Services.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Blog.Services.Api.Extensions;

public static class ServiceExtension
{
    public static void AddServices(this IServiceCollection services, WebApplicationBuilder builder)
    {
        // --- Ma'lumotlar bazasi: SQLite (fayl asosida, hech narsa o'rnatish shart emas) ---
        services.AddDbContext<BlogDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

        // --- Repository'lar ---
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IBlogRepository, BlogRepository>();
        services.AddScoped<IPostRepository, PostRepository>();

        // --- Servislar ---
        services.AddScoped<UserService>();
        services.AddScoped<BlogService>();
        services.AddScoped<PostService>();
        services.AddScoped<JwtTokenService>();
        services.AddScoped<UserHelper>();
        services.AddHttpContextAccessor();

        // --- JwtOption'ni appsettings.json'dan bitta manba sifatida bog'laymiz ---
        // Shu joyda o'qilgan qiymat GenerateToken (JwtTokenService) va token tekshirish
        // (pastdagi AddJwtBearer) uchun bitta xil manbadan keladi - ikkita joyda
        // alohida hardcode qilingan eski xato bartaraf etildi.
        services.Configure<JwtOption>(builder.Configuration.GetSection(nameof(JwtOption)));

        services.AddAuthorization(options =>
        {
            options.AddPolicy("OnlyAdmin", policy => policy.RequireRole("Admin"));
        });

        var jwt = builder.Configuration.GetSection(nameof(JwtOption)).Get<JwtOption>()
                  ?? throw new InvalidOperationException("appsettings.json ichida \"JwtOption\" bo'limi topilmadi");
        var signinKey = Encoding.UTF8.GetBytes(jwt.signinKey);

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = jwt.Issuer,
                ValidAudience = jwt.Audience,
                ValidateIssuer = true,
                ValidateAudience = true,
                IssuerSigningKey = new SymmetricSecurityKey(signinKey),
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });
    }
}
