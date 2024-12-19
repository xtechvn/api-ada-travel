using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels
{
    public class RequestSearchModel
    {
        public string RequestId { get; set; }
        public string SalerId { get; set; }
        public long ClientId { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
