using ENTITIES.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Entities.ViewModels
{
    public class ContractPayViewModel : ContractPay
    {

        public List<ContractPayDetailViewModel> ContractPayDetails { get; set; }
    }

    public class ContractPayViewModelBK : ContractPay
    {
        public string ClientName { get; set; }
        public string ContractPayType { get; set; }
        public string PayDetail { get; set; }
        public string UserName { get; set; }
        public int UserId { get; set; }
        public string TypeStr { get; set; }
        public string PayTypeStr { get; set; }
        public string CreatedByName { get; set; }
        public string BankName { get; set; }
        public string BankAccount { get; set; }
        public double TotalDeposit { get; set; }
        public long TotalRow { get; set; }
        public string PayDetailId { get; set; }
        public IFormFile imagefile { get; set; }
    }
}

