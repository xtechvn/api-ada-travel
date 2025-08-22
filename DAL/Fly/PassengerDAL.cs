using DAL.Generic;
using DAL.StoreProcedure;
using ENTITIES.Models;
using ENTITIES.ViewModels.BookingFly;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Utilities;

namespace DAL.Fly
{
    public class PassengerDAL : GenericService<Passenger>
    {
        private static DbWorker _DbWorker;
        public PassengerDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }

        public List<Passenger> GetPassengers(long orderId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.Passenger.AsNoTracking().Where(s => s.OrderId == orderId).ToList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetPassengers - PassengerDAL: " + ex);
                return null;
            }
        }
        public static int CreatePassengers(PassengerViewModel passenger)
        {
            try
            {

                SqlParameter[] objParam_order = new SqlParameter[8];
                objParam_order[0] = new SqlParameter("@Name", passenger.Name);
                if (passenger.MembershipCard != null)
                {
                    objParam_order[1] = new SqlParameter("@MembershipCard", passenger.MembershipCard);
                }
                else
                {
                    objParam_order[1] = new SqlParameter("@MembershipCard", DBNull.Value);
                }
                objParam_order[2] = new SqlParameter("@PersonType", passenger.PersonType);
                if (passenger.Birthday != null)
                {
                    objParam_order[3] = new SqlParameter("@Birthday", passenger.Birthday);
                }
                else
                {
                    objParam_order[3] = new SqlParameter("@Birthday", DBNull.Value);
                }
                objParam_order[4] = new SqlParameter("@Gender", passenger.Gender);
                objParam_order[5] = new SqlParameter("@OrderId", passenger.OrderId);
                if (passenger.Note != null && passenger.Note.Trim() != "")
                {
                    objParam_order[6] = new SqlParameter("@Note", passenger.Note);
                }
                else
                {
                    objParam_order[6] = new SqlParameter("@Note", DBNull.Value);
                }
                objParam_order[7] = new SqlParameter("@GroupBookingId", passenger.GroupBookingId);

                var id = _DbWorker.ExecuteNonQuery(ProcedureConstants.CreatePassengers, objParam_order);
                return id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreatePassengers - PassengerDAL: " + ex);
                return -1;
            }
        }
    }
}
