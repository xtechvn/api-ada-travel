using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Contants
{
    public class DateUtil
    {
        public static DateTime? Parse(string dateStr, string dateFormat = "yyyy/MM/dd")
        {
            try
            {
                return DateTime.ParseExact(dateStr, dateFormat, null);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string DateToString(DateTime? date, string dateFormat = "yyyy/MM/dd")
        {
            try
            {
                return date == null ? "" : ((DateTime)date).ToString(dateFormat);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string DateTimeToString(DateTime? date, string dateFormat = "yyyy/MM/dd HH:mm:ss")
        {
            try
            {
                return date == null ? "" : ((DateTime)date).ToLocalTime().ToString(dateFormat);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static DateTime? StringToDate(string dateStr, string dateFormat = "yyyy/MM/dd")
        {
            try
            {
                return DateTime.SpecifyKind(DateTime.ParseExact(dateStr, dateFormat, null), DateTimeKind.Local);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static DateTime? StringToDateTime(string dateStr, string dateFormat = "yyyy/MM/dd HH:mm:ss")
        {
            try
            {
                return DateTime.SpecifyKind(DateTime.ParseExact(dateStr, dateFormat, null), DateTimeKind.Local);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
