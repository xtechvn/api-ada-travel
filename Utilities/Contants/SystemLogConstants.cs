using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Contants
{
    //-- Quy định nguồn log
    public enum SystemLogSourceID
    {
        //-- Project CMS
        BACKEND = 0,

        //-- Project Frontend B2B
        FRONTEND_B2B = 1,

        //-- Project Frontend B2C
        FRONTEND_B2C = 2,

        //-- Project API_CORE
        API_CORE = 3,

        //-- Project WIN - APP
        APP = 4,

        //-- Group checkout đơn hàng:
        BOT_CHECKOUT = 5,

        //-- Group bot tiền về:
        BOT_READ_MESSAGE = 6    
    }
    public static class SystemLogTypeID
    {
        public static string ERROR = "error";
        public static string WARNING = "warning";
        public static string BOOKING = "booking";
        public static string ACTIVITY = "activity";
        public static string ERROR_BY_ENVIRONMENT = "error_environment";

    }

}
