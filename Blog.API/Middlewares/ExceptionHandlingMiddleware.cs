using System.Text.Json;
using Blog.Common.Exceptions;

namespace Blog.API.Middlewares;

// Eski kodda HAR BIR controller metodida bir xil try/catch takrorlanardi va
// har qanday xatolik BadRequest (400) qilib qaytarilardi (topilmadi, ruxsat yo'q,
// server xatosi - hammasi bir xil kod bilan). Endi bitta joyda, to'g'ri HTTP
// statuslar bilan boshqariladi, controllerlar esa toza va qisqa qoladi.
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var (statusCode, message) = ex switch
            {
                NotFoundException => (StatusCodes.Status404NotFound, ex.Message),
                BadRequestException => (StatusCodes.Status400BadRequest, ex.Message),
                ForbiddenException => (StatusCodes.Status403Forbidden, ex.Message),
                _ => (StatusCodes.Status500InternalServerError, "Serverda kutilmagan xatolik yuz berdi")
            };

            if (statusCode == StatusCodes.Status500InternalServerError)
                _logger.LogError(ex, "Kutilmagan xatolik: {Path}", context.Request.Path);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = message }));
        }
    }
}
