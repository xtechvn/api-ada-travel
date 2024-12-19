using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ENTITIES.Models
{
    public partial class PolicyDetail
    {
        public int Id { get; set; }
        public int PolicyId { get; set; }
        public int? ClientType { get; set; }
        public int? DebtType { get; set; }
        public decimal? ProductFlyTicketDebtAmount { get; set; }
        public decimal? HotelDebtAmout { get; set; }
        public decimal? ProductFlyTicketDepositAmount { get; set; }
        public decimal? HotelDepositAmout { get; set; }
        public decimal? VinWonderDebtAmount { get; set; }
        public decimal? TourDebtAmount { get; set; }
        public decimal? TouringCarDebtAmount { get; set; }
        public decimal? VinWonderDepositAmount { get; set; }
        public decimal? TourDepositAmount { get; set; }
        public decimal? TouringCarDepositAmount { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
