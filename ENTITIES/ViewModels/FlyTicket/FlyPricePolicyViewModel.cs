using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.FlyTicket
{
    public class FlyPricePolicyViewModel
    {
        public string CampaignCode { get; set; }

        public int PriceDetailId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public double Profit { get; set; }
        public short UnitId { get; set; }
    }
}
