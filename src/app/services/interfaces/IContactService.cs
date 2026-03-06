using backend.app.dtos.request.contact;
using backend.app.dtos.responses.contact;

namespace backend.app.services.interfaces
{
    public interface IContactService
    {
        Task<ContactResponse> SubmitAsync(ContactRequest request);
        Task<ContactResponse?> GetByIdAsync(int id);
        Task<IEnumerable<ContactResponse>> GetAllAsync();
    }
}
