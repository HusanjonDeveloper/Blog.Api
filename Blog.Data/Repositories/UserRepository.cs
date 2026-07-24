using Blog.Common.Exceptions;
using Blog.Data.Context;
using Blog.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Blog.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly BlogDbContext _context;

        public UserRepository(BlogDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetAll()
        {
            return await _context.Users.AsNoTracking().ToListAsync();
        }

        public async Task<User> GetById(Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user is null) throw new NotFoundException($"\"{id}\" identifikatorli foydalanuvchi topilmadi");
            return user;
        }

        public async Task<User?> GetByUsername(string username)
        {
            // Username har doim kichik harfda saqlanadi (UserService.AddUser'da),
            // shuning uchun qidiruvda ham kichik harfga o'tkazamiz - katta/kichik harf farqi bo'lmaydi.
            var normalized = username.ToLower();
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == normalized);
        }

        public async Task Add(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task Update(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}
