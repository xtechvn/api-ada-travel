using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ENTITIES.Models
{
    public partial class CampaignGroupProduct
    {
        public int Id { get; set; }
        public int GroupProductId { get; set; }
        public int CampaignId { get; set; }
        public int Status { get; set; }
        public int? OrderBox { get; set; }
    }
}
