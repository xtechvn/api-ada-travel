using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.ViewModels.Ticket
{
    // DTO kết quả trả ra cho FE
    public class TicketSearchOffersViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }

        public decimal PriceAdult { get; set; }
        public decimal PriceChild { get; set; }
        public decimal PriceSenior { get; set; }

        // Nếu SP có TotalPrice thì map vào (nếu không có thì không dùng)
        public decimal? TotalPrice { get; set; }
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
