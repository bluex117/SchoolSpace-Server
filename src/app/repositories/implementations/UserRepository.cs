using backend.app.configurations.resources;
using backend.app.models.core;
using backend.app.repositories.interfaces;

using Microsoft.EntityFrameworkCore;

namespace backend.app.repositories.implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDatabaseContext _context;

        public UserRepository(AppDatabaseContext context)
        {
            _context = context;
        }

        public async Task<User> CreateUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> UpdateUserAsync(int id, User updated)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return null;

            user.Password = updated.Password ?? user.Password;
            user.Usertype = updated.Usertype;
            user.Name = updated.Name ?? user.Name;
            user.Username = updated.Username ?? user.Username;
            user.AvatarUrl = updated.AvatarUrl ?? user.AvatarUrl;
            user.Phone = updated.Phone ?? user.Phone;

            await _context.SaveChangesAsync();
            return user;
        }

        /// Partial update: only assigned properties are updated. All properties use the same
        /// pattern (updated.X is { } x) for consistency; for value types (e.g. Status, Usertype)
        /// this is always true but keeps behavior consistent and supports nullable value types later.
        public async Task<User?> UpdatePartialAsync(User updated)
        {
            var existing = await _context.Users.FindAsync(updated.Id);
            if (existing == null)
                return null;

            if (updated.Email is { } email)
                existing.Email = email;
            if (updated.Password is { } password)
                existing.Password = password;
            if (updated.Usertype is { } usertype)
                existing.Usertype = usertype;
            if (updated.Name is { } name)
                existing.Name = name;
            if (updated.Username is { } username)
                existing.Username = username;
            if (updated.AvatarUrl is { } avatarUrl)
                existing.AvatarUrl = avatarUrl;
            if (updated.Phone is { } phone)
                existing.Phone = phone;
            if (updated.Status is { } status)
                existing.Status = status;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<User?> UpdateProviderIdsAsync(int id, string? googleId, string? microsoftId)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return null;

            if (googleId != null)
                user.GoogleId = googleId;
            if (microsoftId != null)
                user.MicrosoftId = microsoftId;

            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

            user.Status = UserStatus.SoftDelete;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<User?> GetUserAsync(int id)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .AsNoTracking()
                .OrderBy(u => u.Id)
                .ToListAsync();
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetUserByMicrosoftIdAsync(string microsoftId)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.MicrosoftId == microsoftId);
        }

        public async Task<User?> GetUserByGoogleIdAsync(string googleId)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.GoogleId == googleId);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<IEnumerable<User>> GetUsersAsync(UserRole role)
        {
            var query = _context.Users
                .AsNoTracking()
                .AsQueryable();

            query = query.Where(u => u.Usertype == role);

            return await query.ToListAsync();
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users
                .AsNoTracking()
                .AnyAsync(u => u.Email == email);
        }

        public async Task<List<User>> GetByIdsAsync(IEnumerable<int> ids)
        {
            var idList = ids.Distinct().ToList();

            if (idList.Count == 0)
                return new List<User>();

            return await _context.Users
                .AsNoTracking()
                .Where(u => idList.Contains(u.Id))
                .ToListAsync();
        }

        public async Task<bool> DeleteUserAsync(User user)
        {
            if (user == null)
                return false;

            var existing = await _context.Users.FindAsync(user.Id);
            if (existing == null)
                return false;

            existing.Status = UserStatus.SoftDelete;
            existing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
