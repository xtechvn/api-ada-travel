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
using Utilities.Contants;

namespace DAL.Fly
{
    public class BaggageDAL : GenericService<Baggage>
    {
        private static DbWorker _DbWorker;
        public BaggageDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }

        public List<Baggage> GetBaggages(List<int> passengerIdList)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.Baggage.AsNoTracking().Where(s => passengerIdList.Contains((int)s.PassengerId)).ToList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetBaggages - BaggageDAL: " + ex);
                return null;
            }
        }
        public static int CreateBaggage(BaggageViewModel baggage)
        {
            try
            {

                SqlParameter[] objParam_baggage = new SqlParameter[14];
                objParam_baggage[0] = new SqlParameter("@Airline", baggage.Airline);
                objParam_baggage[1] = new SqlParameter("@Code", baggage.Code);
                objParam_baggage[2] = new SqlParameter("@Confirmed", baggage.Confirmed);
                objParam_baggage[3] = new SqlParameter("@Currency", baggage.Currency);
                if (baggage.EndPoint != null)
                {
                    objParam_baggage[4] = new SqlParameter("@EndPoint", baggage.EndPoint);
                }
                else
                {
                    objParam_baggage[4] = new SqlParameter("@EndPoint", DBNull.Value);
                }
                objParam_baggage[5] = new SqlParameter("@FlightId", baggage.FlightId);
                objParam_baggage[6] = new SqlParameter("@Leg", baggage.Leg);
                objParam_baggage[7] = new SqlParameter("@Name", baggage.Name);
                objParam_baggage[8] = new SqlParameter("@PassengerId", baggage.PassengerId);
                objParam_baggage[9] = new SqlParameter("@Price", baggage.Price);
                if (baggage.StartPoint != null)
                {
                    objParam_baggage[10] = new SqlParameter("@StartPoint", baggage.StartPoint);
                }
                else
                {
                    objParam_baggage[10] = new SqlParameter("@StartPoint", DBNull.Value);
                }
                if (baggage.StatusCode != null)
                {
                    objParam_baggage[11] = new SqlParameter("@StatusCode", baggage.StatusCode);
                }
                else
                {
                    objParam_baggage[11] = new SqlParameter("@StatusCode", DBNull.Value);
                }
                objParam_baggage[12] = new SqlParameter("@Value", baggage.Value);
                objParam_baggage[13] = new SqlParameter("@WeightValue", baggage.WeightValue);

                var id = _DbWorker.ExecuteNonQuery(StoreProceduresName.CreateBaggage, objParam_baggage);
                return id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateBaggage - BaggageDAL: " + ex);
               
                return -1;
            }
        }
    }
}
