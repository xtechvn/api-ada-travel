namespace API_CORE.Model
{
    public class OnePayModel
    {
        public string hash_key { get; set; }
        public string virtual_payment_client_url { get; set; }
        public string AgainLink { get; set; } //Link trang thanh toán của website trước khi chuyển sang OnePAY
        public string Title { get; set; } // Tiêu đề cổng thanh toán hiển thị trên trình duyệt  của chủ thẻ.
        public string vpc_Locale { get; set; }
        public int vpc_Version { get; set; }
        public string vpc_Command { get; set; }
        public string vpc_Merchant { get; set; }
        public string vpc_AccessCode { get; set; }
        public string vpc_MerchTxnRef { get; set; }
        public string vpc_OrderInfo { get; set; }
        public string vpc_Amount { get; set; }
        public string vpc_ReturnURL { get; set; }

        // Thong tin them ve khach hang. De trong neu khong co thong tin
        public string vpc_SHIP_Street01 { get; set; }
        public string vpc_SHIP_Provice { get; set; }
        public string vpc_SHIP_City { get; set; }
        public string vpc_SHIP_Country { get; set; }
        public string vpc_Customer_Phone { get; set; }
        public string vpc_Customer_Email { get; set; }
        public string vpc_Customer_Id { get; set; } // Dia chi IP cua khach hang
        public string vpc_TicketNo { get; set; }
    }
}
