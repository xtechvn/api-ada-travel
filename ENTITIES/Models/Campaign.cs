using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ENTITIES.Models
{
    public partial class Campaign
    {
        public Campaign()
        {
            ProductFlyTicketService = new HashSet<ProductFlyTicketService>();
            RunningScheduleService = new HashSet<RunningScheduleService>();
            VinWonderPricePolicy = new HashSet<VinWonderPricePolicy>();
        }

        public int Id { get; set; }
        public string CampaignCode { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int? ClientTypeId { get; set; }
        public string Description { get; set; }
        public byte Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateLast { get; set; }
        public long UserUpdateId { get; set; }
        public int UserCreateId { get; set; }
        public short? ContractType { get; set; }

        public virtual ICollection<ProductFlyTicketService> ProductFlyTicketService { get; set; }
        public virtual ICollection<RunningScheduleService> RunningScheduleService { get; set; }
        public virtual ICollection<VinWonderPricePolicy> VinWonderPricePolicy { get; set; }
    }
}
