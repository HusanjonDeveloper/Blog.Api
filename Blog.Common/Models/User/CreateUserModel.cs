using System.ComponentModel.DataAnnotations;

namespace Blog.Common.Models.User
{
    public class CreateUserModel
    {
        [Required]
        public string? Firstname { get; set; }
        [Required]
        public string? Lastname { get; set; }
        [Required]
        public string? Username { get; set; }

        [Required]
        public string? Password { get; set; }
      
    }
}
