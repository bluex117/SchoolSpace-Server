using backend.app.models.core;
using backend.app.attributes.repository;

namespace backend.app.repositories.interfaces
{
    [RetryOnTransientFailure]
    public interface IContactRepository
    {
        Task<Contact> CreateAsync(Contact contact);
        Task<Contact?> GetByIdAsync(int id);
        Task<IEnumerable<Contact>> GetAllAsync();
    }
}
