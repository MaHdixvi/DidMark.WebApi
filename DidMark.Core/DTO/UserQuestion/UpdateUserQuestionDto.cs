using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.Core.DTO.Faq
{
    public class UpdateUserQuestionDto
    {
        public long Id { get; set; }
        public string Answer { get; set; }
        public bool IsAnswered { get; set; }
    }

}
