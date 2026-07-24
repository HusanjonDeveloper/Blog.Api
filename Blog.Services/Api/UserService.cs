using Blog.Common.Dtos;
using Blog.Common.Exceptions;
using Blog.Common.Models.User;
using Blog.Common.Statics;
using Blog.Data.Entities;
using Blog.Data.Repositories;
using Blog.Services.Api.Extensions;
using Microsoft.AspNetCore.Identity;

namespace Blog.Services.Api
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtTokenService _jwtTokenService;

        public UserService(IUserRepository userRepository, JwtTokenService jwtTokenService)
        {
            _userRepository = userRepository;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<List<UserDto>> GetAllUsers()
        {
            var users = await _userRepository.GetAll();
            return users.ParsToModels();
        }

        public async Task<UserDto> GetUserById(Guid id)
        {
            var user = await _userRepository.GetById(id);
            return user.ParsToModel();
        }

        // Ro'yxatdan o'tgandan so'ng darhol token qaytaramiz - frontend uchun qulay
        // (foydalanuvchi ro'yxatdan o'tgach yana alohida login qilishi shart emas).
        public async Task<AuthResponseDto> AddUser(CreateUserModel model)
        {
            var normalizedUsername = model.Username!.ToLower();
            await EnsureUsernameIsFree(normalizedUsername);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Firstname = model.Firstname!,
                Lastname = model.Lastname!,
                Username = normalizedUsername,
                Role = ConsString.UserRole
            };

            user.PasswordHash = new PasswordHasher<User>().HashPassword(user, model.Password!);
            await _userRepository.Add(user);

            var token = _jwtTokenService.GenerateToken(user);
            return ToAuthResponse(user, token);
        }

        public async Task<AuthResponseDto> Login(LoginUserModel model)
        {
            var user = await _userRepository.GetByUsername(model.UserName!.ToLower());

            // Xavfsizlik uchun: "username topilmadi" va "parol xato" holatlarida
            // bir xil umumiy xabar qaytaramiz - aks holda tashqi odam qaysi username
            // ro'yxatdan o'tganini bilib olishi mumkin (username enumeration hujumi).
            if (user is null)
                throw new BadRequestException("Login yoki parol noto'g'ri");

            var result = new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, model.Password!);
            if (result == PasswordVerificationResult.Failed)
                throw new BadRequestException("Login yoki parol noto'g'ri");

            var token = _jwtTokenService.GenerateToken(user);
            return ToAuthResponse(user, token);
        }

        public async Task<UserDto> UpdateUser(Guid userId, UpdateUserModel model)
        {
            var user = await _userRepository.GetById(userId);
            var changed = false;

            if (!string.IsNullOrWhiteSpace(model.Firstname))
            {
                user.Firstname = model.Firstname;
                changed = true;
            }

            if (!string.IsNullOrWhiteSpace(model.Lastname))
            {
                user.Lastname = model.Lastname;
                changed = true;
            }

            if (!string.IsNullOrWhiteSpace(model.Username))
            {
                var normalized = model.Username.ToLower();
                if (normalized != user.Username)
                {
                    await EnsureUsernameIsFree(normalized);
                    user.Username = normalized;
                }
                changed = true;
            }

            if (changed) await _userRepository.Update(user);
            return user.ParsToModel();
        }

        public async Task<string> DeleteUser(Guid userId)
        {
            var user = await _userRepository.GetById(userId);
            await _userRepository.Delete(user);
            return "Foydalanuvchi muvaffaqiyatli o'chirildi";
        }

        private async Task EnsureUsernameIsFree(string normalizedUsername)
        {
            var existing = await _userRepository.GetByUsername(normalizedUsername);
            if (existing is not null)
                throw new BadRequestException($"\"{normalizedUsername}\" foydalanuvchi nomi allaqachon band");
        }

        private static AuthResponseDto ToAuthResponse(User user, string token) => new()
        {
            Token = token,
            UserId = user.Id,
            Username = user.Username,
            Firstname = user.Firstname,
            Lastname = user.Lastname
        };
    }
}
