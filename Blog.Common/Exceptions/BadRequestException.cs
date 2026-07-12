namespace Blog.Common.Exceptions;

// Foydalanuvchi xatosi uchun (masalan: username band, parol noto'g'ri, validatsiya xatosi).
// Global middleware buni HTTP 400 statusiga aylantiradi.
public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message)
    {
    }
}