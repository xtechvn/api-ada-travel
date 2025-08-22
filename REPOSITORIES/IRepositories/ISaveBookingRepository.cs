using ENTITIES.ViewModels.BookingFly;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace REPOSITORIES.IRepositories
{
    public interface ISaveBookingRepository
    {
        Task<long> saveBookingAda(BookingFlyMua_Di data);
    }
}
