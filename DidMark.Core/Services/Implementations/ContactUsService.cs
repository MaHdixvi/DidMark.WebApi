using DidMark.Core.Security;
using DidMark.Core.Security.ContactUs;
using DidMark.Core.Services.Interfaces;
using DidMark.DataLayer.Entities.Site;
using DidMark.DataLayer.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.Core.Services.Implementations
{
    public class ContactUsService : IContactUs
    {
        #region constructor
        private IGenericRepository<Contact> userRepository;

        public ContactUsService(IGenericRepository<Contact> userRepository)
        {
            this.userRepository = userRepository;

        }
        #endregion

        public Task<List<Contact>> GetAllContact()
        {
            throw new NotImplementedException();
        }

        public async Task<ContactResult> SendMessage(ContactUsDTO contact)
        {
            var contactUs = new Contact
            {
                FullName = contact.FullName.SanitizeText(),
                Email = contact.Email.SanitizeText(),
                PhoneNumber = contact.PhoneNumber.SanitizeText(),
                Title = contact.Title.SanitizeText(),
                Description = contact.Description.SanitizeText()
            };

            await userRepository.AddEntity(contactUs);

            await userRepository.SaveChanges();



            return ContactResult.Success;
        }

        #region dispose

        public void Dispose()
        {
            userRepository?.Dispose();
        }
        #endregion
    }
}
