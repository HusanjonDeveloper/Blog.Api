using Blog.Common.Dtos;
using Blog.Common.Exceptions;
using Blog.Common.Models.Blog;
using Blog.Data.Repositories;
using Blog.Services.Api.Extensions;

namespace Blog.Services.Api
{
    public class BlogService
    {
        private readonly IBlogRepository _blogRepository;
        private readonly IUserRepository _userRepository;

        public BlogService(IBlogRepository blogRepository, IUserRepository userRepository)
        {
            _blogRepository = blogRepository;
            _userRepository = userRepository;
        }

        // Tizimdagi barcha bloglar (ochiq, umumiy feed - login shart emas)
        public async Task<List<BlogDto>> GetAllBlogs()
        {
            var blogs = await _blogRepository.GetAll();
            return blogs.ParseModels();
        }

        public async Task<BlogDto> GetBlogById(int blogId)
        {
            var blog = await _blogRepository.GetById(blogId);
            return blog.ParseToModel();
        }

        // Faqat shu foydalanuvchiga tegishli bloglar
        public async Task<List<BlogDto>> GetUserBlogs(Guid userId)
        {
            await EnsureUserExists(userId);
            var blogs = await _blogRepository.GetByUserId(userId);
            return blogs.ParseModels();
        }

        public async Task<BlogDto> AddBlog(Guid userId, CreateBlogModel model)
        {
            await EnsureUserExists(userId);
            await EnsureNameIsFree(model.Name);

            var blog = new Data.Entities.Blog
            {
                Name = model.Name,
                Description = model.Description,
                UserId = userId
            };
            await _blogRepository.Add(blog);
            return blog.ParseToModel();
        }

        public async Task<BlogDto> UpdateBlog(Guid userId, int blogId, UpdateBlogModel model)
        {
            var blog = await GetOwnedBlog(userId, blogId);
            var changed = false;

            if (!string.IsNullOrWhiteSpace(model.Name) && model.Name != blog.Name)
            {
                await EnsureNameIsFree(model.Name);
                blog.Name = model.Name;
                changed = true;
            }

            if (!string.IsNullOrWhiteSpace(model.Description))
            {
                blog.Description = model.Description;
                changed = true;
            }

            if (changed) await _blogRepository.Update(blog);
            return blog.ParseToModel();
        }

        public async Task<string> DeleteBlog(Guid userId, int blogId)
        {
            var blog = await GetOwnedBlog(userId, blogId);
            await _blogRepository.Delete(blog);
            return "Blog muvaffaqiyatli o'chirildi";
        }

        // Blogni to'g'ridan-to'g'ri o'zining ID'si bo'yicha bazadan olib,
        // keyin egasi shu userId ekanligini tekshiradi.
        // ESLATMA: eski kodda bu "user.Blogs.FirstOrDefault(...)" orqali qilingan edi,
        // lekin user.Blogs hech qachon yuklanmagani uchun (Include/lazy-loading yo'q edi)
        // u har doim null bo'lib, funksiya butunlay ishlamas edi. Endi bevosita
        // blogni ID orqali olib, egalikni tekshiramiz - ishonchli va tezroq usul.
        internal async Task<Data.Entities.Blog> GetOwnedBlog(Guid userId, int blogId)
        {
            var blog = await _blogRepository.GetById(blogId);
            if (blog.UserId != userId)
                throw new NotFoundException($"\"{userId}\" foydalanuvchiga tegishli \"{blogId}\" IDli blog topilmadi");
            return blog;
        }

        private async Task EnsureUserExists(Guid userId) => await _userRepository.GetById(userId);

        private async Task EnsureNameIsFree(string name)
        {
            var existing = await _blogRepository.GetByName(name);
            if (existing is not null)
                throw new BadRequestException($"\"{name}\" nomli blog allaqachon mavjud");
        }
    }
}
