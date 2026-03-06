using backend.app.dtos.request.report;
using backend.app.dtos.responses.report;
using backend.app.errors.app;
using backend.app.errors.http;
using backend.app.models.core;
using backend.app.repositories.interfaces;
using backend.app.services.interfaces;

namespace backend.app.services.implementations
{
    public sealed class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;
        private readonly IUserRepository _userRepository;

        public ReportService(IReportRepository reportRepository, IUserRepository userRepository)
        {
            _reportRepository = reportRepository;
            _userRepository = userRepository;
        }

        public async Task<ReportResponse?> CreateAsync(int reportedUserId, string reason, int reporterId)
        {
            if (reporterId == reportedUserId)
                throw new SelfReportException();

            var reportedUser = await _userRepository.GetUserAsync(reportedUserId);
            if (reportedUser is null)
                throw new ResourceNotFoundException("Reported user not found.");

            var report = new Report
            {
                ReporterId = reporterId,
                ReportedUserId = reportedUserId,
                Reason = reason,
                Status = ReportStatus.Pending
            };

            report = await _reportRepository.CreateAsync(report);
            return MapToResponse(report);
        }

        public async Task<ReportResponse?> GetByIdAsync(int id)
        {
            var report = await _reportRepository.GetByIdAsync(id);
            if (report is null)
                throw new ResourceNotFoundException("Report not found.");
            return MapToResponse(report);
        }

        public async Task<IEnumerable<ReportResponse>> GetAllAsync()
        {
            var reports = await _reportRepository.GetAllAsync();
            return reports.Select(MapToResponse);
        }

        public async Task<ReportResponse?> UpdateAsync(int id, UpdateReportRequest request)
        {
            var existing = await _reportRepository.GetByIdAsync(id);
            if (existing is null)
                throw new ResourceNotFoundException("Report not found.");

            var updated = new Report
            {
                Id = existing.Id,
                ReporterId = existing.ReporterId,
                ReportedUserId = existing.ReportedUserId,
                Reason = existing.Reason,
                Status = request.Status ?? existing.Status,
                CreatedAt = existing.CreatedAt,
                UpdatedAt = DateTime.UtcNow
            };

            var result = await _reportRepository.UpdateAsync(updated);
            return MapToResponse(result!);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var deleted = await _reportRepository.DeleteAsync(id);
            if (!deleted)
                throw new ResourceNotFoundException("Report not found.");
            return true;
        }

        private static ReportResponse MapToResponse(Report report)
        {
            return new ReportResponse
            {
                Id = report.Id,
                ReporterId = report.ReporterId,
                ReportedUserId = report.ReportedUserId,
                Reason = report.Reason,
                Status = report.Status,
                CreatedAt = report.CreatedAt,
                UpdatedAt = report.UpdatedAt
            };
        }
    }
}
