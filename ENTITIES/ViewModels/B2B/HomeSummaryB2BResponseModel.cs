using ENTITIES.ViewModels.ElasticSearch;
using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.B2B
{
    public class HomeSummaryB2BResponseModel
    {
        public double total_amount { get; set; } = 0;
        public double total_waiting_payment { get; set; } = 0;
        public double total_payment { get; set; } = 0;
        public int total_order_payment { get; set; } = 0;
        public int total_order_waiting_payment { get; set; } = 0;
        public int total_order_checkin { get; set; } = 0;
        public int total_order_checkout { get; set; } = 0;
        public List<OrderElasticsearchViewModel> list_order { get; set; }
        public List<OrderElasticsearchViewModel> list_order_waiting_payment { get; set; }
        public List<OrderElasticsearchViewModel> list_order_payment { get; set; }
        public List<OrderElasticsearchViewModel> list_order_checkin { get; set; }
        public List<OrderElasticsearchViewModel> list_order_checkout { get; set; }
    }
}
