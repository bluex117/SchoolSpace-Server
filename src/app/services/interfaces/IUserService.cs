using backend.app.dtos.responses.general;

namespace backend.app.services.interfaces
{
    public interface IUserService
    {
        Task<UserResponse?> GetUserByIdAsync(int id);
        Task<IEnumerable<UserResponse>> GetAllUsersAsync();
        Task<UserResponse?> UpdateUserAsync(int id, string? name, string? username, string? phone);
        Task<bool> SoftDeleteUserAsync(int id);
        Task<UserResponse?> UpdateUserAvatarAsync(int id, string avatarUrl);
    }
}
