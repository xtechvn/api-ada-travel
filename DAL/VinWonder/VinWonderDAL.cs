using DAL.Generic;
using DAL.StoreProcedure;
using ENTITIES.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.VinWonder
{
    public class VinWonderDAL:GenericService<HotelBooking>
    {
        private static DbWorker _DbWorker;
        public VinWonderDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }
    }
}
