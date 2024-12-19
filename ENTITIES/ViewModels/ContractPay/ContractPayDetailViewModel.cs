namespace Entities.ViewModels
{
    public class ContractPayDetailViewModel
    {
        public int Id { get; set; }
        public int PayId { get; set; }
        public long OrderId { get; set; }
        public int PayDetailId { get; set; }
        public double Amount { get; set; }
        public double TotalNeedPayment { get; set; }
        public int? CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public string BankName { get; set; }
        public string BankAccount { get; set; }
        public string PaymentTypeStr { get; set; }
    }
     public class ContractPayDetaiByOrderIdlViewModel
    {
       
        public int PayId { get; set; }
        public string BillNo { get; set; }
        public string ExportDate { get; set; }
        public string Note { get; set; }
        public double AmountPay { get; set; }
       
    }
}
