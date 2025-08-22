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
    public class FlightSegmentDAL : GenericService<FlightSegment>
    {
        private static DbWorker _DbWorker;
        public FlightSegmentDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }

        public FlightSegment GetFlyBookingDetailId(long flyBookingDetailId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.FlightSegment.AsNoTracking().FirstOrDefault(s => s.FlyBookingId == flyBookingDetailId);
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetail - OrderDAL: " + ex);
                return null;
            }
        }

        public List<FlightSegment> GetFlyBookingDetailIds(List<long> flyBookingDetailIds)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.FlightSegment.AsNoTracking().Where(s => flyBookingDetailIds.Contains(s.FlyBookingId)).ToList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetail - OrderDAL: " + ex);
                return new List<FlightSegment>();
            }
        }
        public static int CreateFlySegment(FlyingSegmentViewModel fly_segment)
        {
            try
            {
                SqlParameter[] dt_fly_segment = new SqlParameter[22];
                dt_fly_segment[0] = new SqlParameter("@FlyBookingId", fly_segment.FlyBookingId);
                dt_fly_segment[1] = new SqlParameter("@OperatingAirline", fly_segment.OperatingAirline);
                dt_fly_segment[2] = new SqlParameter("@StartPoint", fly_segment.StartPoint);
                dt_fly_segment[3] = new SqlParameter("@EndPoint", fly_segment.EndPoint);
                dt_fly_segment[4] = new SqlParameter("@StartTime", fly_segment.StartTime.LocalDateTime);
                dt_fly_segment[5] = new SqlParameter("@EndTime", fly_segment.EndTime.LocalDateTime);
                dt_fly_segment[6] = new SqlParameter("@FlightNumber", fly_segment.FlightNumber);
                dt_fly_segment[7] = new SqlParameter("@Duration", fly_segment.Duration);
                dt_fly_segment[8] = new SqlParameter("@Class", fly_segment.Class);
                dt_fly_segment[9] = new SqlParameter("@Plane", fly_segment.Plane);
                if (fly_segment.StartTerminal != null)
                {
                    dt_fly_segment[10] = new SqlParameter("@StartTerminal", fly_segment.StartTerminal);
                }
                else
                {
                    dt_fly_segment[10] = new SqlParameter("@StartTerminal", DBNull.Value);
                }
                if (fly_segment.EndTerminal != null)
                {
                    dt_fly_segment[11] = new SqlParameter("@EndTerminal", fly_segment.EndTerminal);
                }
                else
                {
                    dt_fly_segment[11] = new SqlParameter("@EndTerminal", DBNull.Value);
                }
                if (fly_segment.StopPoint != null)
                {
                    dt_fly_segment[12] = new SqlParameter("@StopPoint", fly_segment.StopPoint);
                }
                else
                {
                    dt_fly_segment[12] = new SqlParameter("@StopPoint", DBNull.Value);
                }
                dt_fly_segment[13] = new SqlParameter("@StopTime", fly_segment.StopTime);

                if (fly_segment.AllowanceBaggage != null)
                {
                    dt_fly_segment[14] = new SqlParameter("@AllowanceBaggage", fly_segment.AllowanceBaggage);
                }
                else
                {
                    dt_fly_segment[14] = new SqlParameter("@AllowanceBaggage", DBNull.Value);
                }

                if (fly_segment.HandBaggage != null)
                {
                    dt_fly_segment[15] = new SqlParameter("@HandBaggage", fly_segment.HandBaggage);
                }
                else
                {
                    dt_fly_segment[15] = new SqlParameter("@HandBaggage", DBNull.Value);
                }
                dt_fly_segment[16] = new SqlParameter("@HasStop", fly_segment.HasStop);
                dt_fly_segment[17] = new SqlParameter("@ChangeStation", fly_segment.ChangeStation);
                dt_fly_segment[18] = new SqlParameter("@ChangeAirport", fly_segment.ChangeAirport);
                dt_fly_segment[19] = new SqlParameter("@StopOvernight", fly_segment.StopOvernight);
                dt_fly_segment[20] = new SqlParameter("@AllowanceBaggageValue", fly_segment.AllowanceBaggageValue);
                dt_fly_segment[21] = new SqlParameter("@HandBaggageValue", fly_segment.HandBaggageValue);

                var id = _DbWorker.ExecuteNonQuery(StoreProceduresName.CreateFlySegment, dt_fly_segment);
                return id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetail - OrderDAL: " + ex);
                return -1;
            }
        }
    }
}
