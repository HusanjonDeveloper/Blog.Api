namespace Blog.Common.Exceptions;

// Foydalanuvchi autentifikatsiyadan o'tgan, lekin bu amalni bajarishga ruxsati yo'q
// (masalan, boshqa birovning blogini o'chirmoqchi bo'lsa).
// Global middleware buni HTTP 403 statusiga aylantiradi.
public class ForbiddenException : Exception
{
    public ForbiddenException(string message) : base(message)
    {
    }
}
