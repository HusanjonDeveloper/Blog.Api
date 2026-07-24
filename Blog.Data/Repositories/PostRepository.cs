using Blog.Common.Exceptions;
using Blog.Data.Context;
using Blog.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Blog.Data.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly BlogDbContext _dbContext;

        public PostRepository(BlogDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Post>> GetAll() => await _dbContext.Posts.AsNoTracking().ToListAsync();

        public async Task<Post> GetById(int id)
        {
            var post = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == id);
            if (post is null) throw new NotFoundException($"\"{id}\" identifikatorli post topilmadi");
            return post;
        }

        // Blogga tegishli postlarni to'g'ridan-to'g'ri bazadan (SQL WHERE bilan) oladi.
        public async Task<List<Post>> GetByBlogId(int blogId)
        {
            return await _dbContext.Posts.AsNoTracking().Where(p => p.BlogId == blogId).ToListAsync();
        }

        public async Task Add(Post post)
        {
            _dbContext.Posts.Add(post);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Update(Post post)
        {
            _dbContext.Posts.Update(post);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteById(Post post)
        {
            _dbContext.Posts.Remove(post);
            await _dbContext.SaveChangesAsync();
        }
    }
}
