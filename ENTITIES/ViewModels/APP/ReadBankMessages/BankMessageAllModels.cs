using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.APPModels.ReadBankMessages
{
    public class BankMessageDetail
    {
      public double Amount { get; set; }
      public string BankName { get; set; }
      public string AccountNumber { get; set; }
      public string OrderNo { get; set; }
      public long OrderId { get; set; }
      public string BookingCode { get; set; }
      public DateTime ReceiveTime { get; set; }
      public string MessageContent { get; set; }
      public string ImagePath { get; set; }
      public bool StatusPush { get; set; }
      public int BankTransferType { get; set; }
      public DateTime CreatedTime { get; set; }
      public string TransferDescription { get; set; }
      public string TransferCode { get; set; }
      public int is_specify_transfer_to_order { get; set; }
    }
    public class SMSMessageModel
    {
        public string receiver_name { get; set; }
        public string message_text { get; set; }
        public string message_label { get; set; }
    }
    
}
