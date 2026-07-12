namespace Blog.Common.Dtos
{
    // Login va Register endpointlari shu obyektni qaytaradi.
    // Frontend token bilan birga userId/username'ni ham darhol oladi,
    // JWT'ni dekod qilishga hojat qolmaydi.
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Firstname { get; set; } = string.Empty;
        public string Lastname { get; set; } = string.Empty;
    }
}