using ENTITIES.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.AllotmentFunds
{
    public class AllotmentFundTransferViewModel : AllotmentFund
    {
        public int to_fund_type { get; set; }
        public double amount_move { get; set; }
    }
}
