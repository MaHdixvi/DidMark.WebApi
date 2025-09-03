using DidMark.Core.Security.ContactUs;
using DidMark.DataLayer.Entities.Site;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.Core.Services.Interfaces
{
    public interface IContactUs : IDisposable
    {
        Task<List<Contact>> GetAllContact();

        Task<ContactResult> SendMessage(ContactUsDTO contact);
    }
}
