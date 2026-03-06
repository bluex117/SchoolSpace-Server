using backend.app.configurations.security;
using backend.app.dtos.request.user;
using backend.app.dtos.responses.general;
using backend.app.errors.http;
using backend.app.services.interfaces;
using backend.app.utilities.implementation;
using backend.app.utilities.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.app.implementations.Controllers
{
    [ApiController]
    [Route("users")]
    [Authorize]
    public sealed class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ICustomLogger _logger;

        public UserController(IUserService userService, ICustomLogger logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                return Ok(user);
            }
            catch (Exception e)
            {
                if (e is AppException)
                    return HandleError.Resolve(e);

                Logger.Error($"[UserController] GetById failed: {e}");
                return HandleError.Resolve(e);
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception e)
            {
                if (e is AppException)
                    return HandleError.Resolve(e);

                Logger.Error($"[UserController] GetAll failed: {e}");
                return HandleError.Resolve(e);
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
        {
            try
            {
                if (!CanModifyUser(id))
                    return Forbid();

                var user = await _userService.UpdateUserAsync(id, request.Name, request.Username, request.Phone);
                return Ok(user);
            }
            catch (Exception e)
            {
                if (e is AppException)
                    return HandleError.Resolve(e);

                Logger.Error($"[UserController] Update failed: {e}");
                return HandleError.Resolve(e);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> SoftDelete(int id, CancellationToken cancellationToken)
        {
            try
            {
                if (!CanModifyUser(id))
                    return Forbid();

                await _userService.SoftDeleteUserAsync(id);
                return NoContent();
            }
            catch (Exception e)
            {
                if (e is AppException)
                    return HandleError.Resolve(e);

                Logger.Error($"[UserController] SoftDelete failed: {e}");
                return HandleError.Resolve(e);
            }
        }

        [HttpPatch("{id:int}/avatar")]
        public async Task<IActionResult> UpdateAvatar(int id, [FromBody] UpdateUserAvatarRequest request, CancellationToken cancellationToken)
        {
            try
            {
                if (!CanModifyUser(id))
                    return Forbid();

                var user = await _userService.UpdateUserAvatarAsync(id, request.AvatarUrl);
                return Ok(user);
            }
            catch (Exception e)
            {
                if (e is AppException)
                    return HandleError.Resolve(e);

                Logger.Error($"[UserController] UpdateAvatar failed: {e}");
                return HandleError.Resolve(e);
            }
        }

        private bool CanModifyUser(int targetUserId)
        {
            var payload = User.GetUserPayload();
            return payload.Id == targetUserId || string.Equals(payload.Role, "Admin", StringComparison.OrdinalIgnoreCase);
        }
    }
}
