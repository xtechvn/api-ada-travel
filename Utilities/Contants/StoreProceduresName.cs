using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Contants
{
    public static class StoreProceduresName
    {
        public static string SP_GetHotelOrderByAccountClientId = "SP_GetHotelOrderByAccountClientId";
        public static string InsertHotel = "SP_InsertHotel";
        public static string sp_getProvinceByStreet = "sp_getProvinceByStreet";
        public static string sp_getDistrictByStreet = "sp_getDistrictByStreet";
        public static string sp_getWardByStreet = "sp_getWardByStreet";
        public static string sp_countServiceUse = "sp_countServiceUse";
        public static string GetClientByID = "SP_GetClientByID";
        public static string GetClientByAccountClientID = "SP_GetClientByAccountClientID";
        public static string GetContactClientByID = "SP_GetContactClientByID";
        public static string GetContractByID = "SP_GetContractByID";
        public static string GetFlyBookingDetailByOrderID = "SP_GetFlyBookingDetailByOrderID";
        public static string GetOrderByID = "SP_GetOrderByID";
        public static string GetPassengerByContactClientID = "SP_GetPassengerByContactClientID";
        public static string CreateContactClients = "SP_CreateContactClients";
        public static string CreateFlyBookingDetail = "SP_CreateFlyBookingDetail";
        public static string CreateOrder = "SP_CreateOrder";
        public static string CreatePassengers = "SP_CreatePassengers";
        public static string CreateBaggage = "SP_CreateBaggage";
        public static string CheckIfNewOrderValid = "SP_CheckIfNewOrderValid";
        public static string CreateFlySegment = "SP_CreateFlySegment";
        public static string CreateHotelBooking = "SP_CreateHotelBooking";
        public static string CreateHotelBookingRoomRates = "SP_CreateHotelBookingRoomRates";
        public static string CreateHotelBookingRooms = "SP_CreateHotelBookingRooms";
        public static string CreateHotelGuest = "SP_CreateHotelGuest";
        public static string InsertHotelBookingRoomExtraPackages = "SP_InsertHotelBookingRoomExtraPackages";
        public static string DeleteHotelBooking = "SP_DeleteHotelBooking";
        public static string InsertContract = "SP_InsertContract";
        public static string SP_InsertContractPay = "SP_InsertContractPay";
        public static string SP_UpdateContractPay = "SP_UpdateContractPay";
        public static string SP_InsertContractPayDetail = "SP_InsertContractPayDetail";
        public static string SP_UpdateContractPayDetail = "SP_UpdateContractPayDetail";
        public static string SP_UpdateOrderFinishPayment = "SP_UpdateOrderFinishPayment";
        public static string SP_UpdateDepositFinishPayment = "SP_UpdateDepositFinishPayment";
        public static string sp_GetDetailContract = "sp_GetDetailContract";
        public static string SP_UpdateContractStatus = "SP_UpdateContractStatus";
        public static string SP_InsertContractHistory = "SP_InsertContractHistory";
        public static string sp_GetListContractByClientId = "sp_GetListContractByClientId";
        public static string SP_GetDetailOrderServiceByOrderId = "SP_GetDetailOrderServiceByOrderId";
        public static string sp_GetDetailContractHistory = "sp_GetDetailContractHistory";
        public static string SP_DeleteContract = "SP_DeleteContract";
        public static string SP_UpdateContract = "SP_UpdateContract";
        public static string sp_get_client_by_id = "sp_get_client_by_id";
        public static string sp_GetDetailPolicy = "sp_GetDetailPolicy";
        public static string SP_GetContractPayByOrderId = "SP_GetContractPayByOrderId";
        public static string SP_InsertPolicy = "SP_InsertPolicy";
        public static string SP_InsertPolicyDetail = "SP_InsertPolicyDetail";
        public static string SP_UpdatePolicyDetail = "SP_UpdatePolicyDetail";
        public static string SP_UpdatePolicy = "SP_UpdatePolicy";
        public static string SP_InsertSupplier = "SP_InsertSupplier";
        public static string SP_UpdateSupplier = "SP_UpdateSupplier";
        public static string SP_GetListSupplier = "SP_GetListSupplier";
        public static string SP_DeletePolicy = "SP_DeletePolicy";
        public static string InsertTour = "SP_InsertTour";
        public static string InsertTourPackages = "SP_InsertTourPackages";
        public static string UpdateTourPackages = "SP_UpdateTourPackages";
        public static string UpdateTour = "SP_UpdateTour";
        public static string InsertTourGuests = "SP_InsertTourGuests";
        public static string UpdateTourGuests = "SP_UpdateTourGuests";
        public static string Sp_GetTourByOrderId = "Sp_GetTourByOrderId";
        public static string InsertFlyBookingDetail = "SP_CreateFlyBookingDetail";
        public static string UpdateFlyBookingDetail = "SP_UpdateFlyBookingDetail";
        public static string InsertFlyBookingExtraPackages = "SP_InsertFlyBookingExtraPackages";
        public static string UpdateFlyBookingExtraPackages = "SP_UpdateFlyBookingExtraPackages";
        public static string SP_InsertPaymentRequest = "SP_InsertPaymentRequest";
        public static string SP_UpdatePaymentRequestStatus = "SP_UpdatePaymentRequestStatus";
        public static string SP_InsertPaymentRequestDetail = "SP_InsertPaymentRequestDetail";
        public static string SP_UpdatePaymentRequest = "SP_UpdatePaymentRequest";
        public static string SP_UpdatePaymentRequestDetail = "SP_UpdatePaymentRequestDetail";
        public static string SP_GetDetailHotelBookingByOrderID = "SP_GetDetailHotelBookingByOrderID";
        public static string SP_GetDetailOrderByOrderId = "SP_GetDetailOrderByOrderId";
        public static string SP_GetHotelBookingById = "SP_GetHotelBookingById";
        public static string SP_GetHotelBookingRateByHotelBookingRoomID = "SP_GetHotelBookingRateByHotelBookingRoomID";
        public static string SP_GetHotelBookingRoomByHotelBookingID = "SP_GetHotelBookingRoomByHotelBookingID";
        public static string SP_GetHotelBookingByOrderID = "SP_GetHotelBookingByOrderID";
        public static string SP_GetHotelGuestByOrderId = "SP_GetHotelGuestByOrderId";
        public static string sp_gethotelbookingroomextrapackagebyhotelbookingid = "sp_gethotelbookingroomextrapackagebyhotelbookingid";
        public static string SP_UpdateOrderStatus = "SP_UpdateOrderStatus";
        public static string SP_UpdateHotelBookingStatus = "SP_UpdateHotelBookingStatus";
        public static string SP_UpdateHotelBookingRooms = "SP_UpdateHotelBookingRooms";
        public static string SP_GetAllServiceByOrderId = "SP_GetAllServiceByOrderId";
        public static string SP_GetDetailTourByID = "SP_GetDetailTourByID";
        public static string SP_GetDetailHotelBookingByID = "SP_GetDetailHotelBookingByID";
        public static string SP_GetDetailFlyBookingDetailById = "SP_GetDetailFlyBookingDetailById";
        public static string GetListFlyBooking = "SP_GetListFlyBooking";
        public static string SP_UpdateHotelBookingRoomRate = "SP_UpdateHotelBookingRoomRate";
        public static string sp_GetDetailBookingCodeByHotelBookingId = "sp_GetDetailBookingCodeByHotelBookingId";
        public static string Sp_InsertHotelBookingCode = "Sp_InsertHotelBookingCode";
        public static string Sp_UpdateHotelBookingCode = "Sp_UpdateHotelBookingCode";
        public static string SP_GetDetailBookingCodeById = "SP_GetDetailBookingCodeById";
        public static string SP_InsertPaymentVoucher = "SP_InsertPaymentVoucher";
        public static string SP_UpdatePaymentVoucher = "SP_UpdatePaymentVoucher";
        public static string SP_CountHotelBookingByStatus = "SP_CountHotelBookingByStatus";
        public static string SP_GetListTour = "SP_GetListTour";
        public static string SP_GetListTourPackagesByTourId = "SP_GetListTourPackagesByTourId";
        public static string SP_GetListTourGuestsByTourId = "SP_GetListTourGuestsByTourId";
        public static string SP_UpdateTourStatus = "SP_UpdateTourStatus";
        public static string SP_UpdateHotelBookingRoomExtraPackages = "SP_UpdateHotelBookingRoomExtraPackages";
        public static string SP_CountTourByStatus = "SP_CountTourByStatus";
        public static string SP_UpdateHotelBooking = "SP_UpdateHotelBooking";
        public static string SP_GetListTourProduct = "SP_GetListTourProduct";
        public static string Sp_InsertServiceDeclines = "Sp_InsertServiceDeclines";
        public static string Sp_GetServiceDeclinesByOrderId = "Sp_GetServiceDeclinesByOrderId";
        public static string SP_CheckClientDebt = "SP_CheckClientDebt";
        public static string SP_GetListTourPackagesOptionalByTourId = "SP_GetListTourPackagesOptionalByTourId";
        public static string SP_InsertTourPackagesOptional = "SP_InsertTourPackagesOptional";
        public static string SP_UpdateTourPackagesOptional = "SP_UpdateTourPackagesOptional";
        public static string SP_InsertTourProduct = "SP_InsertTourProduct";
        public static string SP_UpdateTourProduct = "SP_UpdateTourProduct";
        public static string SP_InsertTourDestination = "SP_InsertTourDestination";
        public static string SP_UpdateTourDestination = "SP_UpdateTourDestination";
        public static string SP_UpdateOperatorByOrderid = "SP_UpdateOperatorByOrderid";
        public static string Sp_GetListOrderByAccountClientId = "Sp_GetListOrderByAccountClientId";
        public static string SP_GetVinWonderBookingByBookingId = "SP_GetVinWonderBookingByBookingId";
        public static string SP_GetVinWonderBookingEmailByOrderID = "SP_GetVinWonderBookingEmailByOrderID";
        public static string SP_GetVinWonderBookingByOrderID = "SP_GetVinWonderBookingByOrderID";
        public static string SP_GetVinWonderBookingTicketByBookingID = "SP_GetVinWonderBookingTicketByBookingID";
        public static string SP_GetVinWonderBookingCustomerByBookingId = "SP_GetVinWonderBookingCustomerByBookingId";
        public static string SP_GetDetailTourProductByID = "SP_GetDetailTourProductByID";
        public static string SP_fe_GetListTourProduct = "SP_fe_GetListTourProduct";
        public static string SP_GetDetailOtherBookingById = "SP_GetDetailOtherBookingById";
        public static string SP_fe_GetListFavoriteTourProduct = "SP_fe_GetListFavoriteTourProduct";
        public static string SP_fe_GetListTourByAccountId = "SP_fe_GetListTourByAccountId";


        public static string SP_GetListBankingAccountBySupplierId = "SP_GetListBankingAccountBySupplierId";
        public static string SP_UpdateBankingAccount = "SP_UpdateBankingAccount";
        public static string SP_InsertBankingAccount = "SP_InsertBankingAccount";
        public static string GetListContractPayDetailByOrderId = "SP_GetListContractPayDetailByOrderId";

        public static string SP_fe_GetListHotel = "SP_fe_GetListHotel";
        public static string SP_fe_GetListHotelPosition = "SP_fe_GetListHotelPosition";
        public static string SP_fe_GetHotelRoomByHotelId = "SP_fe_GetHotelRoomByHotelId";
        public static string SP_FE_GetHotelRoomPrice = "SP_FE_GetHotelRoomPrice";
        public static string SP_GetListProgramsPackageByRoomId = "SP_GetListProgramsPackageByRoomId";
        public static string SP_GetListProgramsPackageDailyByRoomId = "SP_GetListProgramsPackageDailyByRoomId";

        public static string Sp_UpdateDebtStatusByPayId = "Sp_UpdateDebtStatusByPayId";
        public static string GetListHotelPricePolicyByHotelID = "SP_GetHotelPricePolicyActiveByHotelID";
        public static string GetListHotelPricePolicyDailyByHotelID = "SP_GetHotelPricePolicyDailyActiveByHotelID";
        public static string SP_GetAllOrder_search = "SP_GetAllOrder_search";
        public static string SP_GetListBankingAccountByAccountClientId = "SP_GetListBankingAccountByAccountClientId";

        public static string SP_InsertHotel = "SP_InsertHotel";
        public static string SP_UpdateHotel = "SP_UpdateHotel";
        public static string SP_InsertHotelRoom = "SP_InsertHotelRoom";
        public static string SP_UpdateHotelRoom = "SP_UpdateHotelRoom";
        public static string SP_DeleteHotelRoom = "SP_DeleteHotelRoom";
        public static string SP_InsertPrograms = "SP_InsertPrograms";
        public static string SP_UpdatePrograms = "SP_UpdatePrograms";
        public static string sp_UpdateProgramPackage = "sp_UpdateProgramPackage";
        public static string sp_InsertProgramPackage = "sp_InsertProgramPackage";
        public static string SP_GetListProgramsPackageExpired = "SP_GetListProgramsPackageExpired";

        public static string SP_GetFlyPricePolicyActive = "SP_GetFlyPricePolicyActive";
        public static string SP_GetHotelRoomByHotelId = "SP_GetHotelRoomByHotelId";

        public static string SP_FE_GetContractPayDepositByClientId = "SP_FE_GetContractPayDepositByClientId";
        public static string SP_FE_GetAllotmentUseByClientId = "SP_FE_GetAllotmentUseByClientId";
        public static string SP_InsertAllotmentUse = "SP_InsertAllotmentUse";

        public static string SP_Report_TotalRevenueOrderByDay = "SP_Report_TotalRevenueOrderByDay";
        public static string SP_GetListAccountClient = "SP_GetListAccountClient";

        public static string SP_GetListNational = "SP_GetListNational";
        public static string SP_GetListProvinces = "SP_GetListProvinces";

        public static string SP_GetListTourProgramPackagesByTourProductId = "SP_GetListTourProgramPackagesByTourProductId";
        public static string SP_fe_SearchHotels = "SP_fe_SearchHotels";
        public static string SP_fe_SearchHotelsPosition = "SP_fe_SearchHotelsPosition";
        public static string SP_fe_GetListTourProductPosition = "SP_fe_GetListTourProductPosition";

        public static string sp_GetTotalAccountBalance = "sp_GetTotalAccountBalance";

        public static string SP_GetUserAgentByClientId = "SP_GetUserAgentByClientId";
        public static string sp_InsertHotelBookingRoomsOptional = "sp_InsertHotelBookingRoomsOptional";
        public static string sp_InsertRequest = "sp_InsertRequest";
        public static string SP_GetListRequest = "SP_GetListRequest";
        public static string sp_UpdateRequest = "sp_UpdateRequest";
        public static string sp_GetDetailRequest = "sp_GetDetailRequest";
        public static string SP_GetListHotelBookingRoomsExtraPackageByBookingId = "SP_GetListHotelBookingRoomsExtraPackageByBookingId";
        public static string SP_GetListHotelSurchargeByHotelId = "SP_GetListHotelSurchargeByHotelId";
        public static string GetListVoucher = "SP_GetListVoucher";
        public const string SP_GetListOrder = "SP_GetListOrder";
        public static string GetListHotelBookingRoomsOptionalByBookingId = "SP_GetListHotelBookingRoomsOptionalByBookingId";
        public static string GetListHotelBookingRoomRatesOptionalByBookingId = "SP_GetListHotelBookingRoomRatesOptionalByBookingId";
    }

}
