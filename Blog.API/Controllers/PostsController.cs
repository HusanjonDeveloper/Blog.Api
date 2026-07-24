using Blog.Common.Exceptions;
using Blog.Common.Models.Post;
using Blog.Services.Api;
using Blog.Services.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly PostService _postService;
        private readonly UserHelper _userHelper;

        public PostsController(PostService postService, UserHelper userHelper)
        {
            _postService = postService;
            _userHelper = userHelper;
        }

        // ---- OCHIQ: umumiy post-feed ----

        [HttpGet("posts")]
        public async Task<IActionResult> GetAllPosts()
        {
            var posts = await _postService.GetAllPosts();
            return Ok(posts);
        }

        [HttpGet("posts/{postId:int}")]
        public async Task<IActionResult> GetPostById(int postId)
        {
            var post = await _postService.GetPostById(postId);
            return Ok(post);
        }

        // ---- Muayyan blogga tegishli postlar ----

        [HttpGet("users/{userId:guid}/blogs/{blogId:int}/posts")]
        public async Task<IActionResult> GetBlogPosts(Guid userId, int blogId)
        {
            var posts = await _postService.GetBlogPosts(userId, blogId);
            return Ok(posts);
        }

        [HttpGet("users/{userId:guid}/blogs/{blogId:int}/posts/{postId:int}")]
        public async Task<IActionResult> GetBlogPostById(Guid userId, int blogId, int postId)
        {
            var post = await _postService.GetBlogPostById(userId, blogId, postId);
            return Ok(post);
        }

        [HttpPost("users/{userId:guid}/blogs/{blogId:int}/posts")]
        [Authorize]
        public async Task<IActionResult> AddPost(Guid userId, int blogId, [FromBody] CreatePostModel model)
        {
            EnsureOwner(userId);
            var post = await _postService.AddPost(userId, blogId, model);
            return Ok(post);
        }

        [HttpPut("users/{userId:guid}/blogs/{blogId:int}/posts/{postId:int}")]
        [Authorize]
        public async Task<IActionResult> UpdatePost(Guid userId, int blogId, int postId, [FromBody] UpdatePostModel model)
        {
            EnsureOwner(userId);
            var post = await _postService.UpdatePost(userId, blogId, postId, model);
            return Ok(post);
        }

        [HttpDelete("users/{userId:guid}/blogs/{blogId:int}/posts/{postId:int}")]
        [Authorize]
        public async Task<IActionResult> DeletePost(Guid userId, int blogId, int postId)
        {
            EnsureOwner(userId);
            var message = await _postService.DeletePost(userId, blogId, postId);
            return Ok(new { message });
        }

        private void EnsureOwner(Guid userId)
        {
            if (_userHelper.UserId != userId)
                throw new ForbiddenException("Faqat o'zingizning postlaringizni boshqarishingiz mumkin");
        }
    }
}
