using Microsoft.EntityFrameworkCore;

namespace Blog.Data.Context
{
    public class BlogDbContext : DbContext
    {
        // Connection string endi Program.cs / ServiceExtension orqali appsettings.json'dan
        // DI konteynerga uzatiladi (AddDbContext). Bu yerda hech qanday hardcoded qiymat yo'q -
        // shu tufayli loyiha istalgan kompyuterda ishlaydi, faqat bitta konkret laptopda emas.
        public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User o'chirilsa - uning bloglari ham o'chadi
            modelBuilder.Entity<Entities.User>()
                .HasMany(u => u.Blogs)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Blog o'chirilsa - uning postlari ham o'chadi
            modelBuilder.Entity<Entities.Blog>()
                .HasMany(b => b.Posts)
                .WithOne(p => p.Blog)
                .HasForeignKey(p => p.BlogId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Entities.User> Users { get; set; }
        public DbSet<Entities.Blog> Blogs { get; set; }
        public DbSet<Entities.Post> Posts { get; set; }
    }
}