using Blog.Common.Exceptions;
using Blog.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Blog.Data.Repositories
{
    public class BlogRepository : IBlogRepository
    {
        private readonly BlogDbContext _dbContext;

        public BlogRepository(BlogDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Entities.Blog>> GetAll()
        {
            return await _dbContext.Blogs.AsNoTracking().ToListAsync();
        }

        public async Task<Entities.Blog> GetById(int id)
        {
            var blog = await _dbContext.Blogs.FirstOrDefaultAsync(b => b.Id == id);
            if (blog is null) throw new NotFoundException($"\"{id}\" identifikatorli blog topilmadi");
            return blog;
        }

        public async Task<Entities.Blog?> GetByName(string name)
        {
            return await _dbContext.Blogs.FirstOrDefaultAsync(b => b.Name.ToLower() == name.ToLower());
        }

        // Foydalanuvchining barcha bloglarini to'g'ridan-to'g'ri bazadan (SQL WHERE bilan) oladi.
        // Eski kodda bu butun jadval xotiraga o'qib olinib, keyin LINQ orqali filtrlanardi - samarasiz edi.
        public async Task<List<Entities.Blog>> GetByUserId(Guid userId)
        {
            return await _dbContext.Blogs.AsNoTracking().Where(b => b.UserId == userId).ToListAsync();
        }

        public async Task Add(Entities.Blog blog)
        {
            _dbContext.Blogs.Add(blog);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Update(Entities.Blog blog)
        {
            _dbContext.Blogs.Update(blog);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Delete(Entities.Blog blog)
        {
            _dbContext.Blogs.Remove(blog);
            await _dbContext.SaveChangesAsync();
        }
    }
}
