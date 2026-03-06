using backend.app.configurations.resources;
using backend.app.models.core;
using backend.app.repositories.interfaces;

using Microsoft.EntityFrameworkCore;

namespace backend.app.repositories.implementations
{
    public class ContactRepository : IContactRepository
    {
        private readonly AppDatabaseContext _context;

        public ContactRepository(AppDatabaseContext context)
        {
            _context = context;
        }

        public async Task<Contact> CreateAsync(Contact contact)
        {
            await _context.Contacts.AddAsync(contact);
            await _context.SaveChangesAsync();
            return contact;
        }

        public async Task<Contact?> GetByIdAsync(int id)
        {
            return await _context.Contacts
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Contact>> GetAllAsync()
        {
            return await _context.Contacts
                .AsNoTracking()
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }
    }
}
