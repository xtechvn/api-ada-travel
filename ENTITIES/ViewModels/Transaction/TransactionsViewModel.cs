using ENTITIES.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.Transaction
{
    public class TransactionsViewModel
    {
        public long ClientId { get; set; }
        public int ServiceType { get; set; }
        public string Amount { get; set; }
        public string ContractNo { get; set; }
        public int Status { get; set; }
        public int UserVerifyId { get; set; }
        public string VerifyDate { get; set; }
        public string BankReference { get; set; }
        public int PaymentType { get; set; }
        public string Description { get; set; }
        public string TransactionNo { get; set; }
    }
    public class TransactionsView
    {
        public int Id { get; set; }
        public DateTime CreateDate { get; set; }
        public string ClientName { get; set; }
        public int Status { get; set; }
        public int PaymentType { get; set; }
        public string TransactionNo { get; set; }
        public string ContractNo { get; set; }
    }
}
