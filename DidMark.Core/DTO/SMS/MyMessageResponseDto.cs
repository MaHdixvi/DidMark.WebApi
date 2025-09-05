using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DidMark.Core.DTO.SMS
{
    [XmlRoot("ArrayOfMessagesBL", Namespace = "http://tempuri.org/")]
    public class MyMessageResponseDto
    {
        [XmlElement("MessagesBL")]
        public List<MessageData> Messages { get; set; }
    }
}
