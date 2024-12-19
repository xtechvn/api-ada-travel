using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.Order
{
    public class OrderViewSearchModel
    {
        public int SysTemType { get; set; } = -1;
        public string PaymentStatus { get; set; }
        public string PermisionType { get; set; }
        public string[] HINHTHUCTT { get; set; }

        public string OrderNo { get; set; }
        public DateTime? StartDateFrom { get; set; }
        public DateTime? StartDateTo { get; set; }
        public DateTime? EndDateFrom { get; set; }
        public DateTime? EndDateTo { get; set; }
        public string Note { get; set; }
        public string UtmSource { get; set; }
        public List<int>? ServiceType { get; set; }
        public List<int>? Status { get; set; }
        public string CreateTime { get; set; }
        public string ToDateTime { get; set; }
        public string CreateName { get; set; }
        public string OperatorId { get; set; }
        public string Sale { get; set; }
        public string SaleGroup { get; set; }
        public string ClientId { get; set; }
        public string SalerPermission { get; set; }
        public int StatusTab { get; set; } = 99;
        public int PageIndex { get; set; }
        public int pageSize { get; set; }
        public string UtmMedium { get; set; }
    }
}
