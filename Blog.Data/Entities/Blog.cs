﻿using System.ComponentModel.DataAnnotations;

namespace Blog.Data.Entities
{
    public class Blog
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public Guid Userid { get; set; }
        public virtual User? User { get; set; }
        public virtual List<Post>? Posts { get; set; }
    }
}
