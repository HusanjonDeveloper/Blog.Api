using Blog.Data.Entities;

namespace Blog.Data.Repositories
{
    public interface IPostRepository
    {
        // GetAll
        // GetById
        //Add
        // Update
        // Delete
        public Task<List<Post>> GetAll();
        public Task<Post> GetById(int id);
        public Task<List<Post>> GetByBlogId(int blogId);
        public Task Add(Post post);
        public Task Update(Post post);
        public Task DeleteById(Post post);
    }
}
