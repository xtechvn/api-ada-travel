using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Newtonsoft.Json;

namespace Entities.ViewModels
{
    public class DataObjectViewModel : Dictionary<string, object>
    {
        public DataObjectViewModel()
        {
        }

        public DataObjectViewModel(object obj)
        {
            string rsstring = JsonConvert.SerializeObject(obj);
            PropertyInfo[] propInfos = obj.GetType().GetProperties();
            foreach (var item in propInfos)
            {
                this[item.Name] = obj.GetType().GetProperty(item.Name).GetValue(obj, null);
            }
        }

        public T ToObject<T>()
        {
            try
            {
                string rsstring = JsonConvert.SerializeObject(this);
                T rs = JsonConvert.DeserializeObject<T>(rsstring);
                return rs;
            }
            catch
            {
                return default(T);
            }

        }
    }
}
