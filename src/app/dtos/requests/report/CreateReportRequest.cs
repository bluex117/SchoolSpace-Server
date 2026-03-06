using System.ComponentModel.DataAnnotations;

namespace backend.app.dtos.request.report
{
    public class CreateReportRequest
    {
        [Required]
        public int ReportedUserId { get; set; }

        [Required]
        [StringLength(2000)]
        public required string Reason { get; set; }
    }
}
