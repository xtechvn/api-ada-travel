using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceReceiverMedia.Common
{
    public class ReadFile
    {
        public static AppSettings LoadConfig()
        {
            using (StreamReader r = new StreamReader("config.json"))
            {
                string json = r.ReadToEnd();
                var result = JsonConvert.DeserializeObject<AppSettings>(json);
                return result;
            }
        }
    }
}
