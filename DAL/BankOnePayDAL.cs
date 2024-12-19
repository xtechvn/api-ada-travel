using DAL.Generic;
using System.Collections.Generic;
using ENTITIES.Models;
using System.Threading.Tasks;
using System.Linq;
using static Utilities.Contants.BankOnePayType;
using static ENTITIES.ViewModels.OnePay.OnePayViewModel;

namespace DAL
{
    public class BankOnePayDAL : GenericService<BankOnePay>
    {
        public BankOnePayDAL(string connection) : base(connection)
        {

        }
        public async Task<List<BankOnePay>> GetAllBankOnePay()
        {
            using (var _DbContext = new EntityDataContext(_connection))
            {
                var data = _DbContext.BankOnePay.Where(s => s.Status == (int)BankOnePayTypeStatus.ACTIVE).ToList();
                
                return data;
            }
        }
    }
}
