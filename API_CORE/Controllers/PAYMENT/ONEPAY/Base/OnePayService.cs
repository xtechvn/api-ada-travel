using API_CORE.Common.OnePay;
using Microsoft.Extensions.Configuration;
using System;

namespace API_CORE.Controllers.PAYMENT.ONEPAY.Base
{
    public class OnePayService
    {
        private readonly IConfiguration configuration;
        private readonly string client_name;
        private readonly string vpc_MerchTxnRef;
        private readonly string order_no;
        private readonly double amount;
        private readonly string return_url;
        private readonly string bank_code;

        private readonly long client_id;
        private readonly string address;
        private readonly string phone;
        private readonly string email;
        public OnePayService(IConfiguration _configuration, string _client_name, string _vpc_MerchTxnRef, string _order_no, double _amount, string _return_url, long _client_id, string _address, string _phone, string _email, string _bank_code)
        {
            configuration = _configuration;
            client_name = _client_name;
            vpc_MerchTxnRef = _vpc_MerchTxnRef;
            order_no = _order_no;
            amount = _amount;
            return_url = _return_url;
            client_id = _client_id;
            address = _address;
            phone = _phone;
            email = _email;
            bank_code = _bank_code;
        }

        public string sendPaymentToOnePay()
        {
            try
            {
                // Khoi tao lop thu vien va gan gia tri cac tham so gui sang cong thanh toan
                var conn = new VPCRequest(configuration["config_onepay:virtual_payment_client_url"]);
                conn.SetSecureSecret(configuration["config_onepay:hash_key"]);
                // Add the Digital Order Fields for the functionality you wish to use
                // Core Transaction Fields
                conn.AddDigitalOrderField("AgainLink", "https://adavigo.com");
                conn.AddDigitalOrderField("Title", "Adavigo");
                conn.AddDigitalOrderField("vpc_Locale", "vn");//Chon ngon ngu hien thi tren cong thanh toan (vn/en)
                conn.AddDigitalOrderField("vpc_Version", "2");
                conn.AddDigitalOrderField("vpc_Command", "pay");
                conn.AddDigitalOrderField("vpc_Merchant", configuration["config_onepay:vpc_merchant"]);
                conn.AddDigitalOrderField("vpc_AccessCode", configuration["config_onepay:vpc_access_code"]);
                conn.AddDigitalOrderField("vpc_MerchTxnRef", vpc_MerchTxnRef.ToString());
                conn.AddDigitalOrderField("vpc_OrderInfo", order_no + "bookingby" + client_name);
                conn.AddDigitalOrderField("vpc_Amount", (amount * 100).ToString());
                conn.AddDigitalOrderField("vpc_ReturnURL", return_url);
                if (!string.IsNullOrEmpty(bank_code))
                {
                    conn.AddDigitalOrderField("vpc_CardList", bank_code);
                }
                // Thong tin them ve khach hang. De trong neu khong co thong tin
                //conn.AddDigitalOrderField("vpc_SHIP_Street01", address);
                //conn.AddDigitalOrderField("vpc_SHIP_Provice", "Hanoi");
                //conn.AddDigitalOrderField("vpc_SHIP_City", "Hanoi");
                //conn.AddDigitalOrderField("vpc_SHIP_Country", "Vietnam");
                //conn.AddDigitalOrderField("vpc_Customer_Phone", phone);
                //conn.AddDigitalOrderField("vpc_Customer_Email", email);
                //conn.AddDigitalOrderField("vpc_Customer_Id", client_id.ToString());
                conn.AddDigitalOrderField("vpc_TicketNo", configuration["config_onepay:vpc_ticket_no"]); // my IP

                // Chuyen huong trinh duyet sang cong thanh toan
                string url = conn.Create3PartyQueryString();
                return url;

            }
            catch (Exception ex)
            {
                return string.Empty;

            }
        }



    }
}
