using backend.app.configurations.resources;
using backend.app.models.core;
using backend.app.repositories.interfaces;

using Microsoft.EntityFrameworkCore;

namespace backend.app.repositories.implementations
{
    public class ReportRepository : IReportRepository
    {
        private readonly AppDatabaseContext _context;

        public ReportRepository(AppDatabaseContext context)
        {
            _context = context;
        }

        public async Task<Report> CreateAsync(Report report)
        {
            await _context.Reports.AddAsync(report);
            await _context.SaveChangesAsync();
            return report;
        }

        public async Task<Report?> GetByIdAsync(int id)
        {
            return await _context.Reports
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Report>> GetAllAsync()
        {
            return await _context.Reports
                .AsNoTracking()
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<Report?> UpdateAsync(Report report)
        {
            var existing = await _context.Reports.FindAsync(report.Id);
            if (existing is null)
                return null;

            existing.Reason = report.Reason;
            existing.Status = report.Status;
            existing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var report = await _context.Reports.FindAsync(id);
            if (report is null)
                return false;

            _context.Reports.Remove(report);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
