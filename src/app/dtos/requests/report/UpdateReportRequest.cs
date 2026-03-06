using backend.app.models.core;

namespace backend.app.dtos.request.report
{
    public class UpdateReportRequest
    {
        public ReportStatus? Status { get; set; }
    }
}
