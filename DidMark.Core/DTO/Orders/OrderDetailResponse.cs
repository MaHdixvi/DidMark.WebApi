using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.Core.DTO.Orders
{
    public class OrderDetailResponse
    {
        public OrderDto Order { get; set; } = new();
        public UserOrderInfoDto UserInfo { get; set; } = new();
    }
}
