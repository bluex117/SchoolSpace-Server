using backend.app.dtos.request.contact;
using backend.app.dtos.responses.contact;
using backend.app.errors.http;
using backend.app.services.interfaces;
using backend.app.utilities.implementation;
using backend.app.utilities.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.app.implementations.Controllers
{
    [ApiController]
    [Route("contact")]
    public sealed class ContactController : ControllerBase
    {
        private readonly IContactService _contactService;
        private readonly ICustomLogger _logger;

        public ContactController(IContactService contactService, ICustomLogger logger)
        {
            _contactService = contactService;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ContactResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Submit([FromBody] ContactRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _contactService.SubmitAsync(request);
                return CreatedAtAction(nameof(Submit), new { id = response.Id }, response);
            }
            catch (Exception e)
            {
                if (e is AppException)
                    return HandleError.Resolve(e);

                _logger.Error($"[ContactController] Submit failed: {e}");
                return HandleError.Resolve(e);
            }
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ContactResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            try
            {
                var contact = await _contactService.GetByIdAsync(id);
                return Ok(contact);
            }
            catch (Exception e)
            {
                if (e is AppException)
                    return HandleError.Resolve(e);

                _logger.Error($"[ContactController] GetById failed: {e}");
                return HandleError.Resolve(e);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(IEnumerable<ContactResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            try
            {
                var contacts = await _contactService.GetAllAsync();
                return Ok(contacts);
            }
            catch (Exception e)
            {
                if (e is AppException)
                    return HandleError.Resolve(e);

                _logger.Error($"[ContactController] GetAll failed: {e}");
                return HandleError.Resolve(e);
            }
        }
    }
}
