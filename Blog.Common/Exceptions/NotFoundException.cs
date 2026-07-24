using Exception = System.Exception;

namespace Blog.Common.Exceptions;

// Resurs (User, Blog, Post) topilmaganda shu exception tashlanadi.
// Global middleware buni HTTP 404 statusiga aylantiradi.
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }
}
