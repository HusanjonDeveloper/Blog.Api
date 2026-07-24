namespace Blog.Data.Repositories
{
    public interface IBlogRepository
    {
        // GetAll
        // GetById
        // GetByName
        //Add
        // Update
        // Delete
        public Task<List<Entities.Blog>> GetAll();
        public Task<Entities.Blog> GetById(int id);
        public Task<Entities.Blog?> GetByName(string name);
        public Task<List<Entities.Blog>> GetByUserId(Guid userId);
        public Task Add(Entities.Blog blog);
        public Task Update(Entities.Blog blog);
        public Task Delete(Entities.Blog blog);
    }
}
