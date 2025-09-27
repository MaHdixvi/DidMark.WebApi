using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.Core.DTO.Faq
{
    public class UserQuestionDto
    {
        public long Id { get; set; }
        public string Question { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime CreateDate { get; set; }
        public bool IsAnswered { get; set; }
        public string Answer { get; set; }
        public string PhoneNumber { get; set; }
    }
}
