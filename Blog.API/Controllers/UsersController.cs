using Blog.Common.Exceptions;
using Blog.Common.Models.User;
using Blog.Services.Api;
using Blog.Services.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly UserHelper _userHelper;

        public UsersController(UserService userService, UserHelper userHelper)
        {
            _userService = userService;
            _userHelper = userHelper;
        }

        // GET api/users -> barcha foydalanuvchilar (login/parol chiqarilmaydi, faqat ochiq ma'lumot)
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsers();
            return Ok(users);
        }

        [HttpGet("{userId:guid}")]
        [Authorize]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            var user = await _userService.GetUserById(userId);
            return Ok(user);
        }

        // POST api/users/register -> ro'yxatdan o'tish, darhol token bilan qaytadi
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateUserModel model)
        {
            var result = await _userService.AddUser(model);
            return Ok(result);
        }

        // POST api/users/login -> token bilan qaytadi
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserModel model)
        {
            var result = await _userService.Login(model);
            return Ok(result);
        }

        [HttpPut("{userId:guid}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UpdateUserModel model)
        {
            // Faqat o'zining profilini o'zgartira oladi
            if (_userHelper.UserId != userId)
                throw new ForbiddenException("Faqat o'zingizning profilingizni o'zgartira olasiz");

            var user = await _userService.UpdateUser(userId, model);
            return Ok(user);
        }

        [HttpDelete("{userId:guid}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            if (_userHelper.UserId != userId)
                throw new ForbiddenException("Faqat o'zingizning profilingizni o'chira olasiz");

            var message = await _userService.DeleteUser(userId);
            return Ok(new { message });
        }
    }
}
