using ENTITIES.Models;
using ENTITIES.ViewModels.Voucher;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace REPOSITORIES.IRepositories
{
    public  interface IVoucherRepository
    {
        Task<Voucher> getDetailVoucher(string voucher_name);
        Task<Voucher> getDetailVoucherbyId(long Id);
        Task<List<VoucherFEModel>> GetVoucherList(long account_client_id, string hotel_id);
    }
}
