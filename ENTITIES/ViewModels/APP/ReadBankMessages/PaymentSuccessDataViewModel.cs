using ENTITIES.Models;
using ENTITIES.ViewModels.APP.ContractPay;
using System;
using System.Collections.Generic;

namespace ENTITIES.ViewModels.APP.ReadBankMessages
{

    public class PaymentSuccessDataViewModel
    {
        public long OrderId { get; set; }
        public long DepositHistoryId { get; set; }
        public long ClientId { get; set; }
        public int ClientType { get; set; }
        public int BankTransferType { get; set; }
        public string OrderNo { get; set; }
        public string ClientName { get; set; }
        public string Email { get; set; }
        public double TotalAmount { get; set; }
        public double TotalPreviousAmount { get; set; }
        public double CurrentAmount { get; set; }
        public DateTime PaymentTime { get; set; }
        public byte? ServiceType { get; set; }
        public long? ContractId { get; set; }
        public long? ContactClientId { get; set; }
        public DateTime? CreatedTime { get; set; }
        public List<string> SessionId { get; set; }
        public string BillNo { get; set; }

    }
    public class OrderPaymentDetailDbViewModel
    {
        public List<ENTITIES.Models.ContractPayDetail> payment { get; set; }
        public ENTITIES.Models.Order order { get; set; }
        public ENTITIES.Models.DepositHistory depositHistory { get; set; }
        public ENTITIES.Models.Client client { get; set; }
        public AccountClient account { get; set; }
        public bool is_payment_exists { get; set; }
    }
}
