using backend.app.configurations.security;
using backend.app.dtos.request.report;
using backend.app.dtos.responses.report;
using backend.app.errors.http;
using backend.app.services.interfaces;
using backend.app.utilities.implementation;
using backend.app.utilities.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.app.implementations.Controllers
{
    [ApiController]
    [Route("reports")]
    [Authorize]
    public sealed class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly ICustomLogger _logger;

        public ReportController(IReportService reportService, ICustomLogger logger)
        {
            _reportService = reportService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReportRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var payload = User.GetUserPayload();
                var report = await _reportService.CreateAsync(request.ReportedUserId, request.Reason, payload.Id);
                return CreatedAtAction(nameof(GetById), new { id = report.Id }, report);
            }
            catch (Exception e)
            {
                if (e is AppException)
                    return HandleError.Resolve(e);

                _logger.Error($"[ReportController] Create failed: {e}");
                return HandleError.Resolve(e);
            }
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            try
            {
                var report = await _reportService.GetByIdAsync(id);
                return Ok(report);
            }
            catch (Exception e)
            {
                if (e is AppException)
                    return HandleError.Resolve(e);

                _logger.Error($"[ReportController] GetById failed: {e}");
                return HandleError.Resolve(e);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            try
            {
                var reports = await _reportService.GetAllAsync();
                return Ok(reports);
            }
            catch (Exception e)
            {
                if (e is AppException)
                    return HandleError.Resolve(e);

                _logger.Error($"[ReportController] GetAll failed: {e}");
                return HandleError.Resolve(e);
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateReportRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var report = await _reportService.UpdateAsync(id, request);
                return Ok(report);
            }
            catch (Exception e)
            {
                if (e is AppException)
                    return HandleError.Resolve(e);

                _logger.Error($"[ReportController] Update failed: {e}");
                return HandleError.Resolve(e);
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            try
            {
                await _reportService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception e)
            {
                if (e is AppException)
                    return HandleError.Resolve(e);

                _logger.Error($"[ReportController] Delete failed: {e}");
                return HandleError.Resolve(e);
            }
        }
    }
}
