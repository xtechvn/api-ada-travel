using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.APP.ContractPay
{
    public class ContractPayDetailSPModel
    {
        public int Id { get; set; }
        public int PayId { get; set; }
        public long? DataId { get; set; }
        public decimal? Amount { get; set; }
        public double ContractPayAmount { get; set; }
    }
}
