using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.APP
{
    public class ResetPasswordModel
    {
        public string token { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string re_password { get; set; }
    }
}
