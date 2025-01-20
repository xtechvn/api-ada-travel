using System;

namespace ENTITIES.ViewModels.ElasticSearch
{
    public class OrderElasticsearchViewModel
    {
        public long id { get; set; }
        public string orderno { get; set; }
        public byte? servicetype { get; set; }
        public DateTime? createtime { get; set; }
        public double? amount { get; set; }
        public int? paymentstatus { get; set; }
        public long? clientid { get; set; }
        public long? contactclientid { get; set; }
        public byte? orderstatus { get; set; }
        public long? contractid { get; set; }
        public string smscontent { get; set; }
        public int? paymenttype { get; set; }
        public string bankcode { get; set; }
        public DateTime? paymentdate { get; set; }
        public string paymentno { get; set; }
        public string colorcode { get; set; }
        public double? discount { get; set; }
        public double? profit { get; set; }
        public DateTime? exprirydate { get; set; }
        public DateTime? startdate { get; set; }
        public DateTime? enddate { get; set; }
        public string productservice { get; set; }
        public string note { get; set; }
        public string utmsource { get; set; }
        public DateTime? updatelast { get; set; }
        public long? salerid { get; set; }
        public string salergroupid { get; set; }
        public long? userupdateid { get; set; }
        public short? systemtype { get; set; }
        public long? accountclientid { get; set; }
        public long? createdby { get; set; }
        public string description { get; set; }
        public short? branchcode { get; set; }
        public string bookinginfo { get; set; }
        public string label { get; set; }
        public short? isfinishpayment { get; set; }
        public int? percentdecrease { get; set; }
        public int? voucherid { get; set; }
        public double? price { get; set; }
        public int? supplierid { get; set; }
        public int? departmentid { get; set; }
        public string operatorid { get; set; }
        public int? userverify { get; set; }
        public DateTime? verifydate { get; set; }
        public int? debtstatus { get; set; }
        public string debtnote { get; set; }
        public double? commission { get; set; }
        public string utmmedium { get; set; }
        public double? refund { get; set; }
        public double? totalfundcustomercare { get; set; }
        public double? amountfinalize { get; set; }
        public DateTime? finalizedate { get; set; }
        public bool? islock { get; set; }
    }
}
