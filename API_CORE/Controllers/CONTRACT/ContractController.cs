using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_CORE.Controllers.CONTRACT
{
    public class ContractController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
