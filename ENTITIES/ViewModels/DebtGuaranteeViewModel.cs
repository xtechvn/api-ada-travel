using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels
{
    public class DebtGuaranteeViewModel
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string ClientName { get; set; }
        public string StatusName { get; set; }
        public string ClientId { get; set; }
        public int Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UserCreateName { get; set; }
        public string Label { get; set; }
        public long OrderId { get; set; }
        public string OrderNo { get; set; }
        public double Amount { get; set; }
        public double Profit { get; set; }
        public double Payment { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

    }
}
