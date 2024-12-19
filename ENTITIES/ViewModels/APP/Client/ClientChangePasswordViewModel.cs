using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.APP.Client
{
  public  class ClientChangePasswordViewModel
    {
        public string Email { get; set; }
        public string PasswordNew { get; set; }
        public string ConfirmPasswordNew { get; set; }
    }
}
