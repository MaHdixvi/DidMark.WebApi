using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.Core.DTO.Newsletter
{
    public class CampaignStatsDTO
    {
        public int TotalCampaigns { get; set; }
        public int SentCampaigns { get; set; }
        public int DraftCampaigns { get; set; }
        public int TotalEmailsSent { get; set; }
        public double AverageOpenRate { get; set; }
        public double AverageClickRate { get; set; }
    }
}
