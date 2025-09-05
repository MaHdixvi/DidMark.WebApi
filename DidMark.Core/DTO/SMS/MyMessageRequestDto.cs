using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.Core.DTO.SMS
{
    class MyMessageRequestDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public int Location { get; set; }
        public string From { get; set; }
        public int Index { get; set; }
        public int Count { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}
