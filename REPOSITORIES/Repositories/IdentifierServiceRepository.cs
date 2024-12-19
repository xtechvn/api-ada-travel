using DAL;
using DAL.Contracts;
using DAL.DepositHistory;
using DAL.Identifier;
using DAL.Orders;
using Entities.ConfigModels;
using Microsoft.Extensions.Options;
using REPOSITORIES.IRepositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace REPOSITORIES.Repositories
{
    public class IdentifierServiceRepository : IIdentifierServiceRepository
    {
        private readonly ContractDAL contractDAL;
        private readonly OrderDAL orderDAL;
        private readonly DepositHistoryDAL depositHistoryDAL;
        private readonly ContractPayDAL contractPayDAL;
        private readonly IdentifierDAL identifierDAL;

        public IdentifierServiceRepository(IOptions<DataBaseConfig> dataBaseConfig)
        {
            contractDAL = new ContractDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            orderDAL = new OrderDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            depositHistoryDAL = new DepositHistoryDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            contractPayDAL = new ContractPayDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            identifierDAL = new IdentifierDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        }
        /// <summary>
        /// Quy tắc 1:
        /// B: phát sinh đơn từ đâu: B2b: B, B2c: C, CMS: O
        // VB: ký hiệu theo dịch vụ. VB: Vé MB, KS: khach sạn, TR: TOUR, Thuê xe: TX
        // 2 số cuối của năm
        // Tháng hiện tại là index tham chiếu sang bảng chữ cái lấy chữ
        // Ngày giao dich 
        // Số thứ tự đơn hàng trong năm
        /// </summary>
        /// <param name="service_type"></param>
        /// <param name="system_type"></param>
        /// <returns></returns>

        public async Task<string> buildOrderNo(int service_type, int system_type)
        {
            string order_no = string.Empty;
            try
            {
                var months = new Dictionary<int, string> { { 1, "A" }, { 2, "B" }, { 3, "C" }, { 4, "D" }, { 5, "E" }, { 6, "F" }, { 7, "G" }, { 8, "H" }, { 9, "K" }, { 10, "L" }, { 11, "M" }, { 12, "N" } };
                var current_date = DateTime.Now;

                //1. phát sinh đơn từ đâu: B2b: B, B2c: C, BACKEND: O (tách)

                switch (system_type)
                {
                    // case (Int16)SourcePaymentType.cms:
                    //order_no += "O";
                    //break;
                    case (Int16)SourcePaymentType.b2b:
                        order_no += "B";
                        break;
                    case (Int16)SourcePaymentType.b2c:
                        order_no += "C";
                        break;
                    default:
                        order_no += "A"; // mặc định sẽ là A ký hiệu Adavigo
                        break;
                }

                //2. VB: ký hiệu theo dịch vụ. VB: Vé MB, KS: khach sạn, TR: TOUR, Thuê xe: TX
                switch (service_type)
                {
                    case (Int16)ServicesType.FlyingTicket:
                        order_no += "VB";
                        break;
                    case (Int16)ServicesType.VINHotelRent:
                        order_no += "KS";
                        break;
                    case (Int16)ServicesType.OthersHotelRent:
                        order_no += "KS";
                        break;
                    case (Int16)ServicesType.VehicleRent:
                        order_no += "TX";
                        break;
                    case (Int16)ServicesType.Tourist:
                        order_no += "TR";
                        break;
                    case (Int16)ServicesType.VinWonderTicket:
                        order_no += "VW";
                        break;
                    default:
                        order_no += "VG";
                        break;
                }

                //3. 2 số cuối của năm
                order_no += current_date.Year.ToString().Substring(current_date.Year.ToString().Length - 2, 2);

                //4. Tháng hiện tại là index tham chiếu sang bảng chữ cái lấy chữ
                order_no += months[current_date.Month];

                //5. Ngày giao dich 
                order_no += current_date.ToString("dd");

                //6. Số thứ tự đơn hàng trong năm.
                long order_count = await orderDAL.CountOrderInYear();

                //6.1: Check số đơn hàng này có chưa
                var order_check = await orderDAL.getOrderNoByOrderNo(order_no + order_count);

                if (!string.IsNullOrEmpty(order_check))
                {
                    //Nếu có rồi tăng lên 1
                    // order_no += (order_count + 1);
                    order_no += string.Format(String.Format("{0,4:0000}", order_count + 1));

                }
                else
                {
                    //order_no += order_count.ToString();
                    order_no += string.Format(String.Format("{0,4:0000}", order_count));


                }

                order_no = string.Format(String.Format("{0,4:0000}", order_no));

                return order_no;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("buildOrderNo - OrderRepository" + ex.ToString() + "service_type = " + service_type + "-- system_type" + system_type);
                //Trả mã random
                var rd = new Random();
                var order_default = rd.Next(DateTime.Now.Day, DateTime.Now.Year) + rd.Next(1, 999);
                order_no = "MB-" + order_default;
                return order_no;
            }
        }

        public async Task<string> buildDepositNo(int service_type)
        {
            string trans_no = string.Empty;
            try
            {
                var current_date = DateTime.Now;
                //1. VB: ký hiệu theo dịch vụ. VB: Vé MB, KS: khach sạn, TR: TOUR, Thuê xe: TX
                switch (service_type)
                {
                    case (Int16)ServicesType.FlyingTicket:
                        trans_no += "VB";
                        break;
                    case (Int16)ServicesType.VINHotelRent:
                        trans_no += "KS";
                        break;
                    case (Int16)ServicesType.OthersHotelRent:
                        trans_no += "KS";
                        break;
                    case (Int16)ServicesType.VehicleRent:
                        trans_no += "TX";
                        break;
                    case (Int16)ServicesType.Tourist:
                        trans_no += "TR";
                        break;
                    default:
                        trans_no += "VG";
                        break;
                }

                //2. 2 số cuối của năm
                trans_no += current_date.Year.ToString().Substring(current_date.Year.ToString().Length - 2, 2);


                //3. Số thứ tự đơn hàng trong năm.
                long trans_count = depositHistoryDAL.CountOrderInYear();

                //3.1 Check số  này có chưa
                // var order_check = await depositHistoryDAL.getDepositHistoryByTransNo(trans_no + trans_count);

                //if (!string.IsNullOrEmpty(order_check))
                //{
                //Nếu có rồi tăng lên 1
                // trans_no += (trans_count + 1);
                // }
                //else
                //{
                trans_no += string.Format(String.Format("{0,5:00000}", trans_count + 1));// trans_count.ToString();
                //}

                return trans_no;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("buildDepositNo - IdentifierServiceRepository" + ex.ToString() + "service_type = " + service_type);
                //Trả mã random
                var rd = new Random();
                var order_default = rd.Next(DateTime.Now.Day, DateTime.Now.Year) + rd.Next(1, 999);
                trans_no = "NAPQUY-" + order_default;
                return trans_no;
            }
        }

        /// <summary>
        /// Mã phiếu thu: PT + 2 ký tự năm tạo + 5 số phía sau tự tăng
        /// </summary>
        /// <param name="service_type"></param>
        /// <returns></returns>
        public async Task<string> buildContractPay()
        {
            string bill_no = string.Empty;
            try
            {
                var current_date = DateTime.Now;
                bill_no = "PT";

                //1. 2 số cuối của năm
                bill_no += current_date.Year.ToString().Substring(current_date.Year.ToString().Length - 2, 2);

                //2. Số thứ tự phiếu thu trong năm.
                long bill_count = contractPayDAL.CountContractPayInYear();

                //format numb
                string s_bill_new = string.Format(String.Format("{0,5:00000}", bill_count + 1));

                //3.1 Check số phiếu thu này có chưa
                var check = await contractPayDAL.getContractPayByBillNo(bill_no + s_bill_new);

                if (!string.IsNullOrEmpty(check))
                {
                    //Nếu có rồi tăng lên 1                 
                    //LogHelper.InsertLogTelegram("buildContractPay - IdentifierServiceRepository" + bill_no + s_bill_new + " đã có. Check lại code");
                    bill_no += string.Format(String.Format("{0,5:00000}", bill_count + 2));
                }
                else
                {
                    bill_no += s_bill_new;
                }

                return bill_no;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("buildContractPay - IdentifierServiceRepository" + ex.ToString());
                //Trả mã random
                var rd = new Random();
                var contract_pay_default = rd.Next(DateTime.Now.Day, DateTime.Now.Year) + rd.Next(1, 999);
                bill_no = "PT-" + contract_pay_default;
                return bill_no;
            }
        }

        /// <summary>
        /// Mã phiếu CHI: PT + 2 ký tự năm tạo + 5 số phía sau tự tăng
        /// </summary>
        /// <param name="service_type"></param>
        /// <returns></returns>
        public async Task<string> BuildPaymentVoucher()
        {
            string bill_no = string.Empty;
            try
            {
                var current_date = DateTime.Now;
                bill_no = "PC";

                //1. 2 số cuối của năm
                bill_no += current_date.Year.ToString().Substring(current_date.Year.ToString().Length - 2, 2);


                //2. Số thứ tự trong năm.
                long bill_count = contractPayDAL.CountPaymentVoucherInYear();

                //format numb
                string s_bill_new = string.Format(String.Format("{0,5:00000}", bill_count + 1));
                
                bill_no += s_bill_new;
                
                return bill_no;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("BuildPaymentVoucher - IdentifierServiceRepository" + ex.ToString());
                //Trả mã random
                var rd = new Random();
                var contract_pay_default = rd.Next(DateTime.Now.Day, DateTime.Now.Year) + rd.Next(1, 999);
                bill_no = "PC-" + contract_pay_default;
                return bill_no;
            }
        }

        /// <summary>
        /// Mã phiếu YEU CAU CHI: PYC + 2 ký tự năm tạo + 5 số phía sau tự tăng
        /// Vẫn thiếu 2 ký tự cuối của năm tạo(vd năm 2023 thì thêm 23 sau YCC)
        //VD :  Sửa mã  yêu cầu chi YCCF00062 thành YCC-23-F-00062
        /// </summary>
        /// <param name="service_type"></param>
        /// <returns></returns>
        public async Task<string> BuildPaymentRequest()
        {
            string bill_no = string.Empty;
            try
            {
                var months = new Dictionary<int, string> { { 1, "A" }, { 2, "B" }, { 3, "C" }, { 4, "D" }, { 5, "E" }, { 6, "F" }, { 7, "G" }, { 8, "H" }, { 9, "K" }, { 10, "L" }, { 11, "M" }, { 12, "N" } };

                var current_date = DateTime.Now;
                bill_no = "YCC";

                // 2 số cuối của năm
                bill_no += current_date.Year.ToString().Substring(current_date.Year.ToString().Length - 2, 2);

                //Tháng hiện tại
                bill_no += months[current_date.Month];

                //2. Số thứ tự đã dùng.
                long bill_count = contractPayDAL.CountPaymentRequest();
                
                //format numb
                string s_bill_new = string.Format(String.Format("{0,5:00000}", bill_count + 1));

                bill_no += s_bill_new;

                return bill_no;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("BuildPaymentRequest - IdentifierServiceRepository" + ex.ToString());
                //Trả mã random
                var rd = new Random();
                var contract_pay_default = rd.Next(DateTime.Now.Day, DateTime.Now.Year) + rd.Next(1, 999);
                bill_no = "PYCC-" + contract_pay_default;
                return bill_no;
            }
        }

        /// <summary>
        /// "O + C"+ 2 ký tự cuối của năm + 5 số tăng tự động
        /// </summary>
        /// <param name="service_type"></param>
        /// <returns></returns>
        public async Task<string> buildOrderNoManual()
        {
            string order_no_manual = string.Empty;
            try
            {
                var months = new Dictionary<int, string> { { 1, "A" }, { 2, "B" }, { 3, "C" }, { 4, "D" }, { 5, "E" }, { 6, "F" }, { 7, "G" }, { 8, "H" }, { 9, "K" }, { 10, "L" }, { 11, "M" }, { 12, "N" } };
                var current_date = DateTime.Now;
                order_no_manual = "A";




                //1. 2 số cuối của năm
                order_no_manual += current_date.Year.ToString().Substring(current_date.Year.ToString().Length - 2, 2);

                //0. Tháng hiện tại là index tham chiếu sang bảng chữ cái lấy chữ
                order_no_manual += months[current_date.Month];

                //2. Số thứ tự  trong năm.
                long order_count = await orderDAL.CountOrderBySystemTypeInYear((Int16)SystemType.offline);

                //format numb
                string s_order_new = string.Format(String.Format("{0,5:00000}", order_count + 1));

                //3.1 Check số này có chưa
                var check = await orderDAL.getOrderNoByOrderNo(order_no_manual + s_order_new);

                if (!string.IsNullOrEmpty(check))
                {
                    //Nếu có rồi tăng lên 1
                    order_no_manual += string.Format(String.Format("{0,5:00000}", order_count + 2));
                    LogHelper.InsertLogTelegram("buildOrderNoManual - IdentifierServiceRepository" + order_no_manual + s_order_new + " đã có. Check lại code");
                }
                else
                {
                    order_no_manual += s_order_new;
                }

                return order_no_manual;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("buildOrderNoManual - IdentifierServiceRepository" + ex.ToString());
                //Trả mã random
                var rd = new Random();
                var contract_pay_default = rd.Next(DateTime.Now.Day, DateTime.Now.Year) + rd.Next(1, 999);
                order_no_manual = "DH-" + contract_pay_default;
                return order_no_manual;
            }
        }

        //Số hợp đồng: HD + 2 số cuối năm tạo tự đông - 4 số cuối
        public async Task<string> buildContractNo()
        {
            string contract_no = string.Empty;
            try
            {
                var current_date = DateTime.Now;
                contract_no = "HD";

                //1. 2 số cuối của năm
                contract_no += current_date.Year.ToString().Substring(current_date.Year.ToString().Length - 2, 2);

                //2. Số thứ tự  trong năm.
                long order_count = await contractDAL.CountContractInYear();

                //format numb
                string s_format = string.Format(String.Format("{0,4:0000}", order_count + 1));

                contract_no += s_format;

                return contract_no;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("buildContractNo - IdentifierServiceRepository" + ex.ToString());
                //Trả mã random
                var rd = new Random();
                var contract_pay_default = rd.Next(DateTime.Now.Day, DateTime.Now.Year) + rd.Next(1, 999);
                contract_no = "ADA-HD-" + contract_pay_default;
                return contract_no;
            }
        }

        //Mã dịch vụ: {service_name} - 4 số cuối
        public async Task<string> buildServiceNo(int service_type)
        {
            string service_name = ServicesType2.service[Convert.ToInt16(service_type)];
            //string service_name = { Convert.ToInt16(ServicesType.OthersHotelRent), "HOTEL" };
            try
            {
                int count = identifierDAL.countServiceUse(service_type);
                //format numb
                string s_format = string.Format(String.Format("{0,4:0000}", count + 1));

                service_name += s_format;

                return service_name;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("buildContractNo - IdentifierServiceRepository" + ex.ToString());
                //Trả mã random
                var rd = new Random();
                var num_default = rd.Next(DateTime.Now.Day, DateTime.Now.Year) + rd.Next(1, 999);
                service_name = service_name + num_default;
                return service_name;
            }
        }

        // sinh mã PHIEU YEU CAU XUAT HOA DON
        //* Mã phiếu được thêm theo nguyên tắc: "YCXHD" + 2 ký tự cuối của năm tạo + 1 ký tự 
        //của tháng tạo(theo A -> Z, A tương ứng với tháng 1, B tương ứng với tháng 2,...) + 5 số tăng tự động
        //Ví dụ: YCXHD23A00004
        public async Task<string> BuildExportBillNo()
        {
            string bill_no = string.Empty;
            try
            {
                var months = new Dictionary<int, string> { { 1, "A" }, { 2, "B" }, { 3, "C" }, { 4, "D" }, { 5, "E" }, { 6, "F" }, { 7, "G" }, { 8, "H" }, { 9, "K" }, { 10, "L" }, { 11, "M" }, { 12, "N" } };

                var current_date = DateTime.Now;
                bill_no = "YCXHD";

                // 2 số cuối của năm
                bill_no += current_date.Year.ToString().Substring(current_date.Year.ToString().Length - 2, 2);

                //Tháng hiện tại
                bill_no += months[current_date.Month];

                //2. Số thứ tự đã dùng.
                long bill_count = contractPayDAL.CountInvoiceRequest();

                //format numb
                string s_bill_new = string.Format(String.Format("{0,5:00000}", bill_count + 1));

                bill_no += s_bill_new;

                return bill_no;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("BuildExportBillNo - IdentifierServiceRepository" + ex.ToString());
                //Trả mã random
                var rd = new Random();
                var contract_pay_default = rd.Next(DateTime.Now.Day, DateTime.Now.Year) + rd.Next(1, 999);
                bill_no = "PYCXHD-" + contract_pay_default;
                return bill_no;
            }
        }

        // Hàm sinh mã chung theo cơ chế
        //* Mã phiếu được thêm theo nguyên tắc: "{mã}" + 2 ký tự cuối của năm tạo + 1 ký tự 
        //của tháng tạo(theo A -> Z, A tương ứng với tháng 1, B tương ứng với tháng 2,...) + 5 số tăng tự động
        //Ví dụ: YCXHD23A00004........
        public async Task<string> BuildRule1(string prefix, int code_type)
        {
            string code_no = string.Empty;
            try
            {
                var months = new Dictionary<int, string> { { 1, "A" }, { 2, "B" }, { 3, "C" }, { 4, "D" }, { 5, "E" }, { 6, "F" }, { 7, "G" }, { 8, "H" }, { 9, "K" }, { 10, "L" }, { 11, "M" }, { 12, "N" } };

                var current_date = DateTime.Now;
                code_no = prefix;

                // 2 số cuối của năm
                code_no += current_date.Year.ToString().Substring(current_date.Year.ToString().Length - 2, 2);

                //Tháng hiện tại
                code_no += months[current_date.Month];

                //2. Số thứ tự đã dùng.

                long bill_count = identifierDAL.CountIdentity(code_type);

                //format numb
                string s_bill_new = string.Format(String.Format("{0,5:00000}", bill_count + 1));

                code_no += s_bill_new;

                return code_no;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("BuildRule1 - IdentifierServiceRepository" + ex.ToString());
                //Trả mã random
                var rd = new Random();
                var contract_pay_default = rd.Next(DateTime.Now.Day, DateTime.Now.Year) + rd.Next(1, 999);
                code_no = prefix + contract_pay_default;
                return code_no;
            }
        }

        public async Task<string> buildClientNo(int code_type, int client_type)
        {
            string code = ClientTypeName.service[Convert.ToInt16(client_type)];
            
            try
            {
                var current_date = DateTime.Now;
                int count = identifierDAL.countClientTypeUse(client_type);
                
                //so tu tang
                string s_format = string.Format(String.Format("{0,5:00000}", count + 1));
                
                //1. 2 số cuối của năm
                string two_year_last = current_date.Year.ToString().Substring(current_date.Year.ToString().Length - 2, 2);

                code = code + two_year_last + s_format;

                return code;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("buildClientNo - IdentifierServiceRepository" + ex.ToString());
                //Trả mã random
                var rd = new Random();
                var num_default = rd.Next(DateTime.Now.Day, DateTime.Now.Year) + rd.Next(1, 999);
                code = code + num_default;
                return code;
            }
        }

        public async Task<string> buildRequest()
        {
            string code = string.Empty;
            try
            {
                int Month = Convert.ToInt32(DateTime.Now.Month.ToString());
                int Year = Convert.ToInt32(DateTime.Now.Year.ToString());
                int day = Convert.ToInt32(DateTime.Now.Day.ToString());
                var datetime_now = new DateTime(Year, Month, day);
                var todate_order = datetime_now.AddHours(23).AddMinutes(59).AddSeconds(59);
                int count = identifierDAL.CountRequestByDay(datetime_now, todate_order);

                //so tu tang
                
                string s_format = string.Format(String.Format("{0,5:00000}", count + 1));

                //1. 2 số cuối của năm
                var current_date = DateTime.Now;
                string two_year_last = current_date.Year.ToString().Substring(current_date.Year.ToString().Length - 2, 2);

                code = "R" + day + Month + two_year_last + s_format;

                return code;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("buildRequest - IdentifierServiceRepository" + ex.ToString());
                //Trả mã random
                var rd = new Random();
                var num_default = rd.Next(DateTime.Now.Day, DateTime.Now.Year) + rd.Next(1, 999);
                code = code + num_default;
                return code;
            }
        }
    }
}
