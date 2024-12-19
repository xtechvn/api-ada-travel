using ENTITIES.Models;
using System.Threading.Tasks;

namespace REPOSITORIES.IRepositories
{
    public  interface IVoucherRepository
    {
        Task<Voucher> getDetailVoucher(string voucher_name);
        Task<Voucher> getDetailVoucherbyId(long Id);
    }
}
