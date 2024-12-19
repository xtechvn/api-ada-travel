using ENTITIES.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.DepositHistory
{
    public class DepositHistoryViewMdel : ENTITIES.Models.DepositHistory
    {
        public string PaymentTypeName { get; set; }
        public string StatusName { get; set; }
        public string ServiceName { get; set; }
        public double Amount { get; set; }
        public int TotalRow { get; set; }
    }
    public class AmountDeposit
    {
        public string fundtypeName { get; set; }

        public List<AllotmentFund> AllotmentFund{ get; set; }
        public List<AllotmentUse> AllotmentUse { get; set; }
    }
    public class AmountServiceDeposit
    {
        public string service_name { get; set; }
        public int id { get; set; }
        public int service_type { get; set; }
        public double account_blance { get; set; }
        public double TotalDebtAmount { get; set; }
        public double TotalAmount { get; set; }



    }
}
