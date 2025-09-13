using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.Core.DTO.OffCodes
{
    public class CreateOffCodeDto
    {
        public string Code { get; set; }
        public int DiscountPercentage { get; set; }
        public DateTime ExpireDate { get; set; }
        public int? MaxUsageCount { get; set; }
    }

}
