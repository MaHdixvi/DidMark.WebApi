using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.Core.DTO.Orders
{
    public class OrderStatsDto
    {
        // آمار کلی
        public int TotalOrders { get; set; }
        public int TotalOrdersThisMonth { get; set; }
        public int TotalOrdersThisWeek { get; set; }
        public int TotalOrdersToday { get; set; }

        // آمار مالی
        public decimal TotalRevenue { get; set; }
        public decimal RevenueThisMonth { get; set; }
        public decimal RevenueThisWeek { get; set; }
        public decimal RevenueToday { get; set; }
        public decimal AverageOrderValue { get; set; }


        // آمار زمانی
        public int PaidOrdersToday { get; set; }


        public List<OrderDto> RecentOrders { get; set; } = new List<OrderDto>();

    }
}
