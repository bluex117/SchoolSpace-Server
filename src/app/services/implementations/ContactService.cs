using backend.app.dtos.request.contact;
using backend.app.dtos.responses.contact;
using backend.app.errors.http;
using backend.app.models.core;
using backend.app.repositories.interfaces;
using backend.app.services.interfaces;

namespace backend.app.services.implementations
{
    public sealed class ContactService : IContactService
    {
        private readonly IContactRepository _contactRepository;

        public ContactService(IContactRepository contactRepository)
        {
            _contactRepository = contactRepository;
        }

        public async Task<ContactResponse> SubmitAsync(ContactRequest request)
        {
            var contact = new Contact
            {
                Email = request.Email,
                Topic = request.Topic,
                Description = request.Description
            };

            contact = await _contactRepository.CreateAsync(contact);

            return MapToResponse(contact);
        }

        public async Task<ContactResponse?> GetByIdAsync(int id)
        {
            var contact = await _contactRepository.GetByIdAsync(id);
            if (contact is null)
                throw new ResourceNotFoundException("Contact not found.");
            return MapToResponse(contact);
        }

        public async Task<IEnumerable<ContactResponse>> GetAllAsync()
        {
            var contacts = await _contactRepository.GetAllAsync();
            return contacts.Select(MapToResponse);
        }

        private static ContactResponse MapToResponse(Contact contact)
        {
            return new ContactResponse
            {
                Id = contact.Id,
                Email = contact.Email,
                Topic = contact.Topic,
                Description = contact.Description,
                CreatedAt = contact.CreatedAt
            };
        }
    }
}
