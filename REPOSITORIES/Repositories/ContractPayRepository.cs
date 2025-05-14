using DAL;
using DAL.Clients;
using DAL.DepositHistory;
using DAL.Fly;
using DAL.Hotel;
using DAL.MongoDB;
using DAL.MongoDB.Flight;
using DAL.Orders;
using Entities.ConfigModels;
using Entities.ViewModels;
using ENTITIES.APPModels.ReadBankMessages;
using ENTITIES.Models;
using ENTITIES.ViewModels.APP.ContractPay;
using ENTITIES.ViewModels.APP.ReadBankMessages;
using ENTITIES.ViewModels.MongoDb;
using iTextSharp.text;
using Microsoft.Extensions.Options;
using REPOSITORIES.IRepositories;
using SharpCompress.Compressors.Xz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;
using static System.Net.Mime.MediaTypeNames;

namespace REPOSITORIES.Repositories
{
    public class ContractPayRepository : IContractPayRepository
    {
        private readonly ContractPayDAL _contractPayDAL;
        private readonly OrderDAL orderDAL;
        private readonly FlyBookingDetailDAL flyBookingDetailDAL;
        private readonly HotelBookingDAL hotelBookingDAL;
        private readonly UserDAL userDAL;
        private readonly DepositHistoryDAL depositHistoryDAL;
        private readonly PaymentDAL paymentDAL;
        private readonly BookingDAL BookingMongoDAL;
        private readonly VoucherDAL VoucherDAL;
        private readonly ClientDAL clientDAL;
        private readonly AccountClientDAL accountClientDAL;
        private static BankingAccountDAL bankingAccountDAL;
        private readonly SMSTransferMongoDAL sMSTransferMongoDAL;

        private readonly string botEmail = "bot@adavigo.com";
        public ContractPayRepository(IOptions<DataBaseConfig> dataBaseConfig)
        {

            _contractPayDAL = new ContractPayDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            orderDAL = new OrderDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            flyBookingDetailDAL = new FlyBookingDetailDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            hotelBookingDAL = new HotelBookingDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            userDAL = new UserDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            depositHistoryDAL = new DepositHistoryDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            paymentDAL = new PaymentDAL(dataBaseConfig.Value.SqlServer.ConnectionString);

            VoucherDAL = new VoucherDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            BookingMongoDAL = new BookingDAL(dataBaseConfig.Value.MongoServer.connection_string, dataBaseConfig.Value.MongoServer.catalog_core);
            sMSTransferMongoDAL = new SMSTransferMongoDAL(dataBaseConfig.Value.MongoServer.connection_string, dataBaseConfig.Value.MongoServer.catalog_core);
            clientDAL = new ClientDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            accountClientDAL = new AccountClientDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            bankingAccountDAL = new BankingAccountDAL(dataBaseConfig.Value.SqlServer.ConnectionString);

        }

        public async Task<PaymentSuccessDataViewModel> UpdateOrderBankTransferPayment(BankMessageDetail detail, string contract_pay_code)
        {
            try
            {

                switch (detail.BankTransferType)
                {
                    case (int)BankMessageTransferType.ORDER_PAYMENT:
                        {
                            return await UpdateOrderPayment(detail, contract_pay_code);
                        }
                    case (int)BankMessageTransferType.DEPOSIT_PAYMENT:
                        {
                            return await UpdateDepositTransfer(detail, contract_pay_code);
                        }
                }
                //var words = detail.TransferDescription.Split(" ");
                //foreach (var w in words)
                //{
                //    if (w == null || w.Trim() == "") { continue; }
                //    //---- Check nếu từ giống với mã đơn
                //    bool is_like_order = false;
                //    foreach (var start_word in order_no_start_with)
                //    {
                //        if (w.ToUpper().Trim().StartsWith(start_word))
                //        {
                //            is_like_order = true;
                //            break;
                //        }
                //    }
                //    if (!is_like_order) { continue; }
                //    var order = orderDAL.GetOrderByOrderNo(w);
                //    if (order != null && order.OrderId > 0)
                //    {
                //        detail.OrderNo = order.OrderNo;
                //        detail.OrderId = order.OrderId;
                //        detail.BankTransferType = (int)BankMessageTransferType.ORDER_PAYMENT;
                //        return await UpdateOrderPayment(detail, contract_pay_code);
                //    }

                //    var deposit = await depositHistoryDAL.GetDepositHistoryByTransNo(w);
                //    if (deposit != null && deposit.Id > 0)
                //    {
                //        detail.OrderNo = deposit.TransNo;
                //        detail.OrderId = deposit.Id;
                //        detail.BankTransferType = (int)BankMessageTransferType.DEPOSIT_PAYMENT;
                //        return await UpdateDepositTransfer(detail, contract_pay_code);
                //    }
                //}

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateOrderBankTransferPayment - OrderRepository" + ex.ToString());
            }
            return null;
        }
        private async Task<PaymentSuccessDataViewModel> UpdateOrderPayment(BankMessageDetail detail, string contract_pay_code,int adavigo_supplier_id=604)
        {
            try
            {
                var db_data = new OrderPaymentDetailDbViewModel()
                {
                    payment = new List<ContractPayDetail>(),
                    account = new AccountClient(),
                    client = new Client(),
                    order = new Order(),
                    is_payment_exists = false
                };

                db_data.order = orderDAL.GetDetail(detail.OrderId);
                if (db_data.order == null || db_data.order.OrderId <= 0)
                {
                //    LogHelper.InsertLogTelegram("UpdateOrderBankTransferPayment - ContractPayAppController -> UpdateOrderPayment "
                //                       + "CANNOT get by OrderID [" + detail.OrderId + "] . Get by [" + detail.OrderNo + "]");
                    db_data.order = orderDAL.GetOrderByOrderNo(detail.OrderNo);
                }
               
                if (db_data.order == null || db_data.order.OrderId <= 0)
                {
                //    LogHelper.InsertLogTelegram("UpdateOrderBankTransferPayment - ContractPayAppController -> UpdateOrderPayment "
                //                      + "Get by OrderNo [" + detail.OrderNo + "] return NULL. Get in message");
                    var words = detail.TransferDescription.Split(" ");
                    foreach (var w in words)
                    {
                        List<string> order_no_start_with = new List<string>() { "CVB", "BKS", "O", "CVW", "KS", "VB", "TR", "VW", "A", "P" };

                        if (w == null || w.Trim() == "") { continue; }
                        //---- Check nếu từ giống với mã đơn
                        bool is_like_order = false;
                        foreach (var start_word in order_no_start_with)
                        {
                            if (w.ToUpper().Trim().StartsWith(start_word))
                            {
                                is_like_order = true;
                                break;
                            }
                        }
                        if (!is_like_order) { continue; }
                        db_data.order = orderDAL.GetOrderByOrderNo(w);
                        if (db_data.order != null && db_data.order.OrderId > 0)
                        {
                            //LogHelper.InsertLogTelegram("UpdateOrderBankTransferPayment - ContractPayAppController -> UpdateOrderPayment "
                            //          + "Match [" + w + "] -> [" + db_data.order.OrderId + "]");
                            break;
                        }
                    }
                }
                if (db_data.order == null || db_data.order.OrderId <= 0) {
                    //LogHelper.InsertLogTelegram("UpdateOrderBankTransferPayment - ContractPayAppController -> UpdateOrderPayment "
                    //                     + "Not Match Either [" + detail.OrderId + "]  [" + detail.OrderNo + "] [" + detail.TransferDescription + "]");
                    return null;
                }
                int previous_order_status = Convert.ToInt32((db_data.order.OrderStatus==null)?0: db_data.order.OrderStatus);

                //-- Get Previous ContractPay:
                db_data.payment = orderDAL.GetContractPayByOrderID(db_data.order.OrderId);
                var previous_amount = db_data.payment.Sum(x => x.Amount == null ? 0 : Convert.ToDouble(x.Amount));
                var banking = new ENTITIES.Models.BankingAccount();
                try
                {
                    DataTable data = bankingAccountDAL.GetBankAccountDataTableBySupplierId(adavigo_supplier_id);
                    var listData = data.ToList<ENTITIES.Models.BankingAccount>();
                    List<ENTITIES.Models.BankingAccount> regex = new List<ENTITIES.Models.BankingAccount>();
                    if (listData != null && listData.Count > 0)
                    {

                        foreach (var b in listData)
                        {
                            if (b.AccountNumber == null || b.AccountNumber.Trim() == "") continue;
                            var account_number = b.AccountNumber.Trim();
                            //^[9*][6*][9*][8*][8*][8*][8*]$
                            char[] chars = account_number.ToCharArray();
                            string regex_item = "^";
                            for (var i = 0; i < chars.Length; i++)
                            {
                                regex_item += "[" + chars[i] + "*x]";
                            }
                            regex_item += "$";
                            regex.Add(new ENTITIES.Models.BankingAccount
                            {
                                Id = b.Id,
                                AccountNumber = regex_item
                            });
                        }
                    }
                    banking = regex.FirstOrDefault(x => Regex.IsMatch(detail.AccountNumber, x.AccountNumber));
                }catch(Exception ex)
                {
                    LogHelper.InsertLogTelegram("UpdateOrderPayment - OrderRepository - Regex Match Banking Account ["+detail.AccountNumber+"]" + ex.ToString());

                }
                if (banking==null || banking.Id<=0) banking = bankingAccountDAL.GetByAccountNumber(detail.BankName, detail.AccountNumber);
                //Check if exists
                var exists_payment = await orderDAL.GetOrderPayment(db_data.order.OrderId, detail.Amount, detail.ReceiveTime);
                if (exists_payment == null || exists_payment.Id <= 0)
                {
                    db_data.is_payment_exists = false;
                    if (db_data.order.PaymentStatus != 1)
                    {
                        if ((previous_amount + detail.Amount) >= db_data.order.Amount)
                        {
                          
                            db_data.order.DebtStatus = (int)DebtStatus.PAID;
                            db_data.order.PaymentStatus = (int)PaymentStatus.PAID;
                            db_data.order.IsFinishPayment = (int)PaymentStatus.PAID;

                        }
                        else
                        {
                            db_data.order.PaymentStatus = (int)PaymentStatus.PAID_NOT_ENOUGH;
                            db_data.order.DebtStatus = (int)DebtStatus.PAID_NOT_ENOUGH;
                            db_data.order.IsFinishPayment = (int)PaymentStatus.PAID_NOT_ENOUGH;

                        }
                    }
                    
                    exists_payment = new Payment()
                    {
                        Amount = detail.Amount,
                        CreatedOn = DateTime.Now,
                        ModifiedOn = DateTime.Now,
                        Note = "Bot tự động thu tiền đơn hàng " + db_data.order.OrderNo + " lúc " + detail.ReceiveTime.ToString("dd/MM/yyyy HH:mm:ss"),
                        TransferContent = detail.ReceiveTime.ToString("dd/MM/yyyy HH:mm:ss") + ". Order " + detail.OrderNo + " +" + detail.Amount,
                        ClientId = (int)db_data.order.ClientId,
                        OrderId = db_data.order.OrderId,
                        PaymentDate = detail.ReceiveTime,
                        PaymentType = (int)PaymentType.CHUYEN_KHOAN_TRUC_TIEP,
                        DepositPaymentType = (short)detail.BankTransferType,
                        BotPaymentScreenShot = detail.ImagePath == null ? "" : detail.ImagePath,
                        BankName = detail.BankName == null ? "" : detail.BankName,
                        Id = 0,
                        ImageScreenShot = ""
                    };
                    var payment_id = paymentDAL.CreatePayment(exists_payment);
                }
                else
                {
                    db_data.is_payment_exists = true;
                }

                db_data.order.PaymentDate = detail.ReceiveTime;
                db_data.order.BankCode = detail.BankName;
                db_data.order.PaymentType = (int)PaymentType.CHUYEN_KHOAN_TRUC_TIEP;
                db_data.order.SmsContent = detail.MessageContent;

                var update_status_service = await orderDAL.UpdateOrderFinishPayment(db_data.order.OrderId, (int)OrderStatus.WAITING_FOR_OPERATOR);

                db_data.client = clientDAL.GetByClientId((long)db_data.order.ClientId);
                db_data.account = await accountClientDAL.GetByID((long)db_data.order.AccountClientId);
                PaymentSuccessDataViewModel model = new PaymentSuccessDataViewModel()
                {
                    ClientName = db_data.client.ClientName,
                    ClientType = (int)(db_data.account ==null || db_data.account.ClientType==null || db_data.account.ClientType<=0?5: db_data.account.ClientType),
                    CurrentAmount = detail.Amount,
                    OrderId = db_data.order.OrderId,
                    OrderNo = db_data.order.OrderNo,
                    Email = db_data.client.Email,
                    PaymentTime = detail.ReceiveTime,
                    TotalAmount = (double)db_data.order.Amount,
                    TotalPreviousAmount = db_data.payment.Sum(x => x.Amount != null ? Convert.ToDouble(x.Amount) : 0),
                    ClientId = db_data.client.Id,
                    ServiceType = db_data.order.ServiceType,
                    ContractId = db_data.order.ContractId,
                    ContactClientId = db_data.order.ContactClientId ==null || db_data.order.ContactClientId <= 0 ? 0 : db_data.order.ContactClientId,
                    CreatedTime = db_data.order.CreateTime,
                    PreviousOrderStatus= previous_order_status
                };
                var user = userDAL.GetByEmail(botEmail);
                var contract_pay_detail = new ContractPayDetailViewModel()
                {
                    OrderId = db_data.order.OrderId,
                    Amount = (detail.Amount > db_data.order.Amount) ? (double)db_data.order.Amount : detail.Amount,
                    CreatedBy = user == null ? 2052 : user.Id,
                };
                
                ContractPayViewModel contract_model = new ContractPayViewModel()
                {
                    BillNo = contract_pay_code,
                    ClientId = Convert.ToInt32(db_data.client.Id),
                    Note = "Thu tiền thanh toán lúc " + detail.ReceiveTime.ToString("dd/MM/yyyy HH:mm"),
                    Amount = detail.Amount,
                    Type = (int)DepositHistoryConstant.CONTRACT_PAY_TYPE.THU_TIEN_DON_HANG,
                    PayType = (int)DepositHistoryConstant.CONTRACT_PAYMENT_TYPE.CHUYEN_KHOAN,
                    BankingAccountId = (banking==null || banking.Id<0)?0:banking.Id,
                    Description = "Bot tự động thu tiền đơn hàng " + db_data.order.OrderNo + " lúc " + detail.ReceiveTime.ToString("dd/MM/yyyy HH:mm:ss"),
                    AttatchmentFile = null,
                    CreatedBy = contract_pay_detail.CreatedBy,
                    ContractPayDetails = new List<ContractPayDetailViewModel>() { contract_pay_detail },
                    DebtStatus=0, 
                    EmployeeId=0,
                    CreatedDate=DateTime.Now,
                    IsDelete=false,
                    ObjectType=0,
                    PayStatus=1,
                    SupplierId=0,
                    
                };
                var contract_data = _contractPayDAL.CreateContractPay(contract_model);
                switch (db_data.order.ServiceType)
                {
                    case (int)ServicesType.FlyingTicket:
                        {
                            var fly = flyBookingDetailDAL.GetListByOrderId(db_data.order.OrderId);
                            if (fly != null && fly.Count > 0)
                            {
                                model.SessionId = fly.Select(x => x.Session).Distinct().ToList();
                            }
                        }
                        break;
                    case (int)ServicesType.VINHotelRent:
                    case (int)ServicesType.OthersHotelRent:
                        {
                            var hotel = hotelBookingDAL.GetListByOrderId(db_data.order.OrderId);
                            if (hotel != null && hotel.Count > 0)
                            {
                                model.SessionId = hotel.Select(x => x.BookingId).Distinct().ToList();
                            }
                        }
                        break;
                }
                if (db_data.order.OrderStatus == (int)OrderStatus.CREATED_ORDER || db_data.order.OrderStatus == (int)OrderStatus.CONFIRMED_SALE)
                {
                    db_data.order.OrderStatus = (int)OrderStatus.WAITING_FOR_OPERATOR;
                }
                await orderDAL.UpdateOrder(db_data.order);

                return model;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateOrderPayment - OrderRepository" + ex.ToString());
            }
            return null;
        }
        public async Task<PaymentSuccessDataViewModel> UpdateDepositTransfer(BankMessageDetail detail,string contract_pay_code,int adavigo_supplier_id=604)
        {
            try
            {
                OrderPaymentDetailDbViewModel db_data = new OrderPaymentDetailDbViewModel()
                {
                    payment = new List<ContractPayDetail>(),
                    account = new AccountClient(),
                    client = new Client(),
                    depositHistory = new DepositHistory(),
                    is_payment_exists = false
                };
                db_data.depositHistory = await depositHistoryDAL.GetDepositHistoryByTransNo(detail.OrderNo);
                if (db_data.depositHistory == null || db_data.depositHistory.Id <= 0) return null;
                var banking = new ENTITIES.Models.BankingAccount();
                try
                {
                    DataTable data = bankingAccountDAL.GetBankAccountDataTableBySupplierId(adavigo_supplier_id);
                    var listData = data.ToList<ENTITIES.Models.BankingAccount>();
                    List<ENTITIES.Models.BankingAccount> regex = new List<ENTITIES.Models.BankingAccount>();
                    if (listData != null && listData.Count > 0)
                    {

                        foreach (var b in listData)
                        {
                            if (b.AccountNumber == null || b.AccountNumber.Trim() == "") continue;
                            var account_number = b.AccountNumber.Trim();
                            //^[9*][6*][9*][8*][8*][8*][8*]$
                            char[] chars = account_number.ToCharArray();
                            string regex_item = "^";
                            for (var i = 0; i < chars.Length; i++)
                            {
                                regex_item += "[" + chars[i] + "*x]";
                            }
                            regex_item += "$";
                            regex.Add(new ENTITIES.Models.BankingAccount
                            {
                                Id = b.Id,
                                AccountNumber = regex_item
                            });
                        }
                    }
                    banking = regex.FirstOrDefault(x => Regex.IsMatch(detail.AccountNumber, x.AccountNumber));
                }
                catch (Exception ex)
                {
                    LogHelper.InsertLogTelegram("UpdateDepositTransfer - OrderRepository - Regex Match Banking Account [" + detail.AccountNumber + "]" + ex.ToString());

                }
                if (banking == null || banking.Id <= 0) banking = bankingAccountDAL.GetByAccountNumber(detail.BankName, detail.AccountNumber);

                var user = userDAL.GetByEmail(botEmail);
                var contract_pay_detail = new ContractPayDetailViewModel()
                {
                    Id = (int)db_data.depositHistory.Id,
                    OrderId = (int)db_data.depositHistory.Id,
                    Amount = detail.Amount > db_data.depositHistory.Price ? (double)db_data.depositHistory.Price : detail.Amount,
                    CreatedBy = user == null ? 2052 : user.Id,
                };
                var upadate_status = await depositHistoryDAL.updateStatusBotVerifyTrans(detail.OrderNo);

                //-- Get Previous ContractPay:
                db_data.payment = orderDAL.GetContractPayByDepositID(db_data.depositHistory.Id);
                var previous_amount = db_data.payment.Sum(x => x.Amount == null ? 0 : Convert.ToDouble(x.Amount));


                db_data.client = clientDAL.GetByClientId((long)db_data.depositHistory.ClientId);
                db_data.account = await accountClientDAL.GetByClientId((long)db_data.depositHistory.UserId);
                db_data.depositHistory = db_data.depositHistory;

                //Check if exists
                var exists_payment = await orderDAL.GetDepositPayment(db_data.depositHistory.Id, detail.Amount, detail.ReceiveTime);
                if (exists_payment == null || exists_payment.Id <= 0)
                {
                    db_data.is_payment_exists = false;
                    exists_payment = new Payment()
                    {
                        Amount = detail.Amount,
                        CreatedOn = DateTime.Now,
                        ModifiedOn = DateTime.Now,
                        Note = "Bot tự động thu tiền ký quỹ " + db_data.depositHistory.TransNo + " lúc " + detail.ReceiveTime.ToString("dd/MM/yyyy HH:mm:ss"),
                        TransferContent = detail.ReceiveTime.ToString("dd/MM/yyyy HH:mm:ss") + ". Transfer No. : " + detail.OrderNo + " . Amount:" + detail.Amount,
                        ClientId = db_data.client.Id,
                        OrderId = db_data.depositHistory.Id,
                        PaymentDate = detail.ReceiveTime,
                        PaymentType = (int)PaymentType.CHUYEN_KHOAN_TRUC_TIEP,
                        DepositPaymentType = (short)detail.BankTransferType,
                        BotPaymentScreenShot = detail.ImagePath == null ? "" : detail.ImagePath,
                        BankName = detail.BankName == null ? "" : detail.BankName,
                        Id = 0,
                        ImageScreenShot = ""
                    };
                    var payment_id = paymentDAL.CreatePayment(exists_payment);
                }
                else
                {
                    db_data.is_payment_exists = true;
                }

                ContractPayViewModel contract_model = new ContractPayViewModel()
                {

                    BillNo = contract_pay_code,
                    ClientId = Convert.ToInt32(db_data.client.Id),
                    Note = "Thu tiền thanh toán lúc " + detail.ReceiveTime.ToString("dd/MM/yyyy HH:mm"),
                    Amount = detail.Amount,
                    Type = (int)DepositHistoryConstant.CONTRACT_PAY_TYPE.THU_TIEN_KY_QUY,
                    PayType = (int)DepositHistoryConstant.CONTRACT_PAYMENT_TYPE.CHUYEN_KHOAN,
                    BankingAccountId = (banking == null || banking.Id < 0) ? 0 : banking.Id,
                    Description = "Bot tự động thu tiền ký quỹ " + db_data.depositHistory.TransNo + " lúc " + detail.ReceiveTime.ToString("dd/MM/yyyy HH:mm:ss"),
                    AttatchmentFile = null,
                    CreatedBy = contract_pay_detail.CreatedBy,
                    ContractPayDetails = new List<ContractPayDetailViewModel>() { contract_pay_detail }
                };
                var contract_data = _contractPayDAL.CreateContractPay(contract_model);
                double total_prev_pay = 0;
                if (db_data != null)
                {
                    if (db_data.payment != null && db_data.payment.Count > 0)
                    {
                        foreach (var prev in db_data.payment)
                        {
                            total_prev_pay += Convert.ToDouble(prev.Amount);
                        }
                    }
                    PaymentSuccessDataViewModel model = new PaymentSuccessDataViewModel()
                    {
                        ClientName = db_data.client.ClientName,
                        ClientType = (int)db_data.client.ClientType,
                        CurrentAmount = detail.Amount,
                        DepositHistoryId = db_data.depositHistory.Id,
                        OrderNo = detail.OrderNo,
                        Email = db_data.client.Email,
                        PaymentTime = detail.ReceiveTime,
                        TotalAmount = (double)db_data.depositHistory.Price,
                        TotalPreviousAmount = total_prev_pay,
                        ClientId = db_data.client.Id,
                        ServiceType = (byte?)db_data.depositHistory.ServiceType,
                        CreatedTime = db_data.depositHistory.CreateDate,
                        BankTransferType = (int)BankMessageTransferType.DEPOSIT_PAYMENT,
                        OrderId = db_data.depositHistory.Id,
                        BillNo = contract_pay_code

                    };
                    return model;
                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateDepositTransfer - ContractPayRepository" + ex.ToString());
            }
            return null;
        }
        public async Task<List<ContractPayDetaiByOrderIdlViewModel>> GetContractPayByOrderId(long OrderId)
        {
            try
            {

                DataTable data = await _contractPayDAL.GetContractPayByOrderId(OrderId);
                var listData = data.ToList<ContractPayDetaiByOrderIdlViewModel>();
                if (listData.Count > 0)
                {
                    return listData;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetContractPayByOrderId - ContractPayDAL. " + ex);
            }
            return null;
        }
        public async Task<List<ENTITIES.Models.BankingAccount>> GetListBankingAccountByAccountClientId(int AccountClientId)
        {
            try
            {

                DataTable data = await bankingAccountDAL.GetListBankingAccountByAccountClientId(AccountClientId);
                var listData = data.ToList<ENTITIES.Models.BankingAccount>();
                if (listData.Count > 0)
                {
                    return listData;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetContractPayByOrderId - ContractPayDAL. " + ex);
            }
            return null;
        }
        
        public async Task<List<ENTITIES.Models.BankingAccount>> GetBankAccountDataTableBySupplierId(int suplier_id)
        {
            try
            {

                DataTable data =  bankingAccountDAL.GetBankAccountDataTableBySupplierId(suplier_id);
                var listData = data.ToList<ENTITIES.Models.BankingAccount>();
                if (listData.Count > 0)
                {
                    return listData;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetBankAccountDataTableBySupplierId - ContractPayDAL. " + ex);
            }
            return null;
        }
        public async Task<string> InsertSMSN8n(SMSN8NMongoModel item)
        {
            return await sMSTransferMongoDAL.Insert(item);
        }
    }
}
