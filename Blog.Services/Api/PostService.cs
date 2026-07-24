using Blog.Common.Dtos;
using Blog.Common.Exceptions;
using Blog.Common.Models.Post;
using Blog.Data.Entities;
using Blog.Data.Repositories;
using Blog.Services.Api.Extensions;

namespace Blog.Services.Api
{
    public class PostService
    {
        private readonly IPostRepository _postRepository;
        private readonly IUserRepository _userRepository;
        private readonly BlogService _blogService;

        public PostService(IPostRepository postRepository, IUserRepository userRepository, BlogService blogService)
        {
            _postRepository = postRepository;
            _userRepository = userRepository;
            _blogService = blogService;
        }

        // Tizimdagi barcha postlar (ochiq umumiy feed)
        public async Task<List<PostDto>> GetAllPosts()
        {
            var posts = await _postRepository.GetAll();
            return posts.ParseModels();
        }

        public async Task<PostDto> GetPostById(int postId)
        {
            var post = await _postRepository.GetById(postId);
            return post.ParseToModel();
        }

        // Muayyan blogga tegishli postlar - to'g'ridan-to'g'ri SQL so'rov bilan (samarali)
        public async Task<List<PostDto>> GetBlogPosts(Guid userId, int blogId)
        {
            await _blogService.GetOwnedBlog(userId, blogId);
            var posts = await _postRepository.GetByBlogId(blogId);
            return posts.ParseModels();
        }

        public async Task<PostDto> GetBlogPostById(Guid userId, int blogId, int postId)
        {
            var post = await CheckPostBelongsToBlog(userId, blogId, postId);
            return post.ParseToModel();
        }

        public async Task<PostDto> AddPost(Guid userId, int blogId, CreatePostModel model)
        {
            var user = await _userRepository.GetById(userId);
            await _blogService.GetOwnedBlog(userId, blogId);

            var post = new Post
            {
                Title = model.Title,
                Content = model.Content,
                AuthorFullName = $"{user.Firstname} {user.Lastname}",
                BlogId = blogId
            };
            await _postRepository.Add(post);
            return post.ParseToModel();
        }

        public async Task<PostDto> UpdatePost(Guid userId, int blogId, int postId, UpdatePostModel model)
        {
            var post = await CheckPostBelongsToBlog(userId, blogId, postId);
            var changed = false;

            if (!string.IsNullOrWhiteSpace(model.Title))
            {
                post.Title = model.Title;
                changed = true;
            }

            if (!string.IsNullOrWhiteSpace(model.Content))
            {
                post.Content = model.Content;
                changed = true;
            }

            if (changed) await _postRepository.Update(post);
            return post.ParseToModel();
        }

        public async Task<string> DeletePost(Guid userId, int blogId, int postId)
        {
            var post = await CheckPostBelongsToBlog(userId, blogId, postId);
            await _postRepository.DeleteById(post);
            return "Post muvaffaqiyatli o'chirildi";
        }

        // Blog userId'ga tegishli ekanligini, keyin post shu blogId'ga tegishli ekanligini tekshiradi.
        private async Task<Post> CheckPostBelongsToBlog(Guid userId, int blogId, int postId)
        {
            await _blogService.GetOwnedBlog(userId, blogId);
            var post = await _postRepository.GetById(postId);
            if (post.BlogId != blogId)
                throw new NotFoundException($"\"{blogId}\" IDli blog ichida \"{postId}\" IDli post topilmadi");
            return post;
        }
    }
}
