using Blog.API.Middlewares;
using Blog.Data.Context;
using Blog.Services.Api.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Blog API", Version = "v1" });

    // To'g'ri JWT sxemasi (eski kodda ApiKey turi ishlatilgan edi - Swagger UI'da
    // "Bearer" so'zini qo'lda yozish kerak bo'lardi; endi Swagger avtomatik qo'shadi).
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Faqat tokenning o'zini kiriting (\"Bearer \" so'zisiz)",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// Barcha repository/servis/DbContext/JWT sozlamalari shu yerda ro'yxatdan o'tadi
builder.Services.AddServices(builder);

// Frontend (oddiy HTML/JS, file:// yoki boshqa port orqali) so'rov yubora olishi uchun.
// Bu yerda faqat Authorization header (token) ishlatilgani uchun (cookie emas),
// AllowAnyOrigin xavfsizlik nuqtai nazaridan muammo emas.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
    });
});

var app = builder.Build();

// Dastur birinchi marta ishga tushganda SQLite fayli (blog.db) va barcha jadvallar
// joriy Entity modellariga (User, Blog, Post) qarab AVTOMATIK yaratiladi.
// EnsureCreated() tanlandi (Migrate() emas) - chunki u "dotnet ef migrations add"
// buyrug'ini oldindan bajarishni talab qilmaydi, shu bilan loyiha hech qanday
// qo'shimcha vositasiz, faqat "dotnet run" bilan darhol ishlaydi.
// ESLATMA: kelajakda jadval strukturasini o'zgartirsangiz (masalan yangi ustun
// qo'shsangiz), eng oddiy yo'l - "blog.db" faylini o'chirib, dasturni qayta
// ishga tushirish (dev bosqichida). Productionga chiqishda esa real EF Core
// migratsiyalariga o'tish tavsiya etiladi.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BlogDbContext>();
    db.Database.EnsureCreated();
}

// Eng birinchi middleware - shundan keyingi hamma narsadagi xatoliklarni ushlaydi
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");

// MUHIM: Authentication har doim Authorization'dan OLDIN kelishi kerak,
// aks holda [Authorize] to'g'ri ishlamaydi. Eski kodda UseAuthentication()
// umuman chaqirilmagan edi (o'lik "Configure" funksiyasi ichida qolib ketgan edi).
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
