using System.Threading.Tasks;
namespace REPOSITORIES.IRepositories
{
    public interface IIdentifierServiceRepository
    {
        Task<string> buildOrderNo(int service_type, int system_type); // sinh mã đơn cho các dịch vụ
        Task<string> buildDepositNo(int service_type); // sinh mã nạp tiền
        Task<string> buildContractPay(); // sinh mã PHIEU THU
        Task<string> buildOrderNoManual();// don thu cong
        Task<string> buildServiceNo(int service_type);// sinh mã dịch vụ
        Task<string> buildContractNo();// sinh mã HD
        Task<string> BuildPaymentVoucher(); // sinh mã PHIEU CHI
        Task<string> BuildPaymentRequest(); // sinh mã PHIEU YEU CAU CHI
        Task<string> BuildExportBillNo(); // sinh mã PHIEU YEU CAU XUAT HOA DON
        Task<string> BuildRule1(string prefix, int code_type); // sinh mã XUAT HOA DON
        Task<string> buildClientNo(int code_type, int client_type); // sinh mã đơn cho khach hang theo loai khach hang
        Task<string> buildRequest(); //sinh mã Request
    }
}
