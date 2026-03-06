using backend.app.dtos.responses.general;
using backend.app.errors.http;
using backend.app.models.core;
using backend.app.repositories.interfaces;
using backend.app.services.interfaces;

namespace backend.app.services.implementations
{
    public sealed class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserResponse?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetUserAsync(id);
            if (user is null)
                throw new ResourceNotFoundException("User not found.");
            return MapToResponse(user);
        }

        public async Task<IEnumerable<UserResponse>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();
            return users.Select(MapToResponse);
        }

        public async Task<UserResponse?> UpdateUserAsync(int id, string? name, string? username, string? phone)
        {
            var user = await _userRepository.GetUserAsync(id);
            if (user is null)
                throw new ResourceNotFoundException("User not found.");

            var updated = new User
            {
                Id = user.Id,
                Email = user.Email,
                Password = user.Password,
                Usertype = user.Usertype,
                Status = user.Status,
                Username = username ?? user.Username,
                Name = name ?? user.Name,
                AvatarUrl = user.AvatarUrl,
                Phone = phone ?? user.Phone,
                MicrosoftId = user.MicrosoftId,
                GoogleId = user.GoogleId,
                CreatedAt = user.CreatedAt,
                UpdatedAt = DateTime.UtcNow
            };

            var result = await _userRepository.UpdatePartialAsync(updated);
            return MapToResponse(result);
        }

        public async Task<bool> SoftDeleteUserAsync(int id)
        {
            var deleted = await _userRepository.DeleteUserAsync(id);
            if (!deleted)
                throw new ResourceNotFoundException("User not found.");
            return true;
        }

        public async Task<UserResponse?> UpdateUserAvatarAsync(int id, string avatarUrl)
        {
            var user = await _userRepository.GetUserAsync(id);
            if (user is null)
                throw new ResourceNotFoundException("User not found.");

            var updated = new User
            {
                Id = user.Id,
                Email = user.Email,
                Password = user.Password,
                Usertype = user.Usertype,
                Status = user.Status,
                Username = user.Username,
                Name = user.Name,
                AvatarUrl = avatarUrl,
                Phone = user.Phone,
                MicrosoftId = user.MicrosoftId,
                GoogleId = user.GoogleId,
                CreatedAt = user.CreatedAt,
                UpdatedAt = DateTime.UtcNow
            };

            var result = await _userRepository.UpdatePartialAsync(updated);
            return MapToResponse(result);
        }

        private static UserResponse MapToResponse(User user)
        {
            return new UserResponse
            {
                Id = user.Id,
                Email = user.Email,
                Usertype = user.Usertype,
                Status = user.Status,
                Username = user.Username,
                Name = user.Name,
                AvatarUrl = user.AvatarUrl,
                Phone = user.Phone,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }
    }
}
