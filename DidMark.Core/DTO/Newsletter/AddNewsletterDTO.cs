using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.Core.DTO.Newsletter
{
    public class AddNewsletterDTO
    {
        [Required, EmailAddress]
        public string Email { get; set; }
    }
}
