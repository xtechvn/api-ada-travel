using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ENTITIES.Models
{
    public partial class ProductClassification
    {
        public int Id { get; set; }
        public int GroupIdChoice { get; set; }
        public string Link { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? Status { get; set; }
        public DateTime? UpdateTime { get; set; }
        public DateTime? CreateTime { get; set; }
        public int? LabelId { get; set; }
        public int? CapaignId { get; set; }
        public int? UserId { get; set; }
        public string Note { get; set; }
        public string LinkProductTarget { get; set; }
        public string ProductName { get; set; }
        public long? ProductId { get; set; }
        public string UtmPath { get; set; }

        public virtual CampaignAds Capaign { get; set; }
        public virtual GroupProduct GroupIdChoiceNavigation { get; set; }
        public virtual Label Label { get; set; }
        public virtual User User { get; set; }
    }
}
