using Blog.Common.Exceptions;
using Blog.Common.Models.Blog;
using Blog.Services.Api;
using Blog.Services.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class BlogsController : ControllerBase
    {
        private readonly BlogService _blogService;
        private readonly UserHelper _userHelper;

        public BlogsController(BlogService blogService, UserHelper userHelper)
        {
            _blogService = blogService;
            _userHelper = userHelper;
        }

        // ---- OCHIQ (login shart emas): umumiy blog-feed ----

        [HttpGet("blogs")]
        public async Task<IActionResult> GetAllBlogs()
        {
            var blogs = await _blogService.GetAllBlogs();
            return Ok(blogs);
        }

        [HttpGet("blogs/{blogId:int}")]
        public async Task<IActionResult> GetBlogById(int blogId)
        {
            var blog = await _blogService.GetBlogById(blogId);
            return Ok(blog);
        }

        // ---- Muayyan foydalanuvchining bloglari ----

        [HttpGet("users/{userId:guid}/blogs")]
        public async Task<IActionResult> GetUserBlogs(Guid userId)
        {
            var blogs = await _blogService.GetUserBlogs(userId);
            return Ok(blogs);
        }

        [HttpPost("users/{userId:guid}/blogs")]
        [Authorize]
        public async Task<IActionResult> AddBlog(Guid userId, [FromBody] CreateBlogModel model)
        {
            EnsureOwner(userId);
            var blog = await _blogService.AddBlog(userId, model);
            return Ok(blog);
        }

        [HttpPut("users/{userId:guid}/blogs/{blogId:int}")]
        [Authorize]
        public async Task<IActionResult> UpdateBlog(Guid userId, int blogId, [FromBody] UpdateBlogModel model)
        {
            EnsureOwner(userId);
            var blog = await _blogService.UpdateBlog(userId, blogId, model);
            return Ok(blog);
        }

        [HttpDelete("users/{userId:guid}/blogs/{blogId:int}")]
        [Authorize]
        public async Task<IActionResult> DeleteBlog(Guid userId, int blogId)
        {
            EnsureOwner(userId);
            var message = await _blogService.DeleteBlog(userId, blogId);
            return Ok(new { message });
        }

        // Token egasi bilan URL'dagi userId bir xil ekanligini tekshiradi -
        // aks holda A foydalanuvchi B nomidan blog yaratishi/o'chirishi mumkin bo'lardi.
        private void EnsureOwner(Guid userId)
        {
            if (_userHelper.UserId != userId)
                throw new ForbiddenException("Faqat o'zingizning bloglaringizni boshqarishingiz mumkin");
        }
    }
}
