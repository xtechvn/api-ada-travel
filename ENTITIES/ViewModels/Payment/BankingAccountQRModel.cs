using ENTITIES.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.Payment
{
    public class BankingAccountQRModel: BankingAccount
    {
        public string Image { get; set; }
        public string Bin { get; set; }
    }
}
