using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_CORE.Model
{
    public class Authentication
    {
        public string authentication_token { get; set; }
        public string refresh_token { get; set; }
        public string token_Type { get; set; }
        public string expires_in { get; set; }
    }
}
