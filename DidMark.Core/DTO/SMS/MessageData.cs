using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.Core.DTO.SMS
{
    public class MessageData
    {
        public int MsgID { get; set; }
        public string Body { get; set; }
        public string SendDate { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
    }
}
