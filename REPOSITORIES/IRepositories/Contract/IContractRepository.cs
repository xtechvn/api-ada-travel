using System.Threading.Tasks;

namespace REPOSITORIES.IRepositories.Contract
{
    public  interface IContractRepository
    {
        Task<long> CountContract();
    }
}
