using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.ViewModels.Ticket
{
    // DTO kết quả trả ra cho FE
    // DTO trả ra FE
    public class TicketSearchOffersViewModel
    {
        public int ProductId { get; set; }
        public int TicketId { get; set; }
        public string ProductName { get; set; }

        // Giá bán đơn vị (base + profit)
        public decimal PriceAdult { get; set; }
        public decimal PriceChild { get; set; }
        public decimal PriceSenior { get; set; }

        // Base đơn vị
        public decimal? BaseAdult { get; set; }
        public decimal? BaseChild { get; set; }
        public decimal? BaseSenior { get; set; }

        // Profit đơn vị
        public decimal? ProfitAdult { get; set; }
        public decimal? ProfitChild { get; set; }
        public decimal? ProfitSenior { get; set; }

        // Tổng theo số lượng
        public decimal? TotalAmount { get; set; }           // base + profit
        public decimal? BaseTotalAmount { get; set; }
        public decimal? ProfitTotalAmount { get; set; }

        public DateTime VisitDate { get; set; }
    }

    // DTO nội bộ (dùng sau khi decode token)
    public class TicketSearchOffersFilter
    {
        public int SupplierId { get; set; }
        public DateTime VisitDate { get; set; }

        public int Adults { get; set; } 
        public int Children { get; set; } 
        public int Seniors { get; set; } 

        public string Search { get; set; }
        public int? CategoryId { get; set; }
        public int? TicketTypeId { get; set; }
        public int? PlayZoneId { get; set; }
        public int? ProductId { get; set; }
    }
}
