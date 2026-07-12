using Blog.Services.Api.Extensions;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Description = "JWT Bearer. : \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
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
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddServices(builder);


void ConfigureServices(IServiceCollection services)
{
    services.AddAuthorization(options =>
     {
         options.AddPolicy("OnlyAdmin", policy =>
         {
             policy.RequireRole("Admin");
         });
     });
    services.AddMvcCore();
}
void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    app.UseAuthentication(); // Authentication middleware
    app.UseAuthorization(); // Authorization middleware
    app.UseMvc();
}

/*
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyCors", policy =>
    {
        policy.AllowAnyHeader();
        policy.AllowCredentials();
        policy.AllowAnyMethod();
        policy.AllowAnyOrigin();
        policy.DisallowCredentials();
    });
});
*/


var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(options =>
{
    options.AllowAnyHeader();
    options.AllowAnyMethod();
    options.AllowAnyOrigin();
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseAuthorization();
app.MapControllers();
app.Run();