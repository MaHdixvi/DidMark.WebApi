using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.Core.DTO.Products
{
    public class ProductVisitDto
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public string UserIp { get; set; } // بهتره IP رو رشته بگیریم تا IPv4 و IPv6 ساپورت بشه
        public DateTime CreateDate { get; set; }
    }
}
