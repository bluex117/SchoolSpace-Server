using backend.app.dtos.request.report;
using backend.app.dtos.responses.report;

namespace backend.app.services.interfaces
{
    public interface IReportService
    {
        Task<ReportResponse?> CreateAsync(int reportedUserId, string reason, int reporterId);
        Task<ReportResponse?> GetByIdAsync(int id);
        Task<IEnumerable<ReportResponse>> GetAllAsync();
        Task<ReportResponse?> UpdateAsync(int id, UpdateReportRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
