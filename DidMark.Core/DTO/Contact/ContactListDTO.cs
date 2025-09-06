using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.Core.DTO.Contact
{
    public class ContactListDTO
    {
        public long Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Title { get; set; }
        public DateTime CreateDate { get; set; }
        public bool IsRead { get; set; }
        public bool IsReplied { get; set; }


    }
}
