using Blog.Data.Entities;

namespace Blog.Data.Repositories
{
    public interface IUserRepository
    {
        // GetAll
        // GetById
        // GetByUserName
        //Add
        // Update
        // Delete

        public Task<List<User>> GetAll();
        public Task<User> GetById(Guid id);
        public Task<User?> GetByUsername(string username);
        public Task Add(User user);
        public Task Update(User user);
        public Task Delete(User user);

    }
}
