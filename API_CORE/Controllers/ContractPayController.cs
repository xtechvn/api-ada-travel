using ENTITIES.ViewModels.ContractPay;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using REPOSITORIES.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace API_CORE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractPayController : Controller
    {

        private IConfiguration configuration;
        private IContractPayRepository contractPayRepository;
        private IIdentifierServiceRepository identifierServiceRepository;

        public ContractPayController(IConfiguration _configuration, IContractPayRepository _contractPayRepository, IIdentifierServiceRepository _identifierServiceRepository)
        {
            configuration = _configuration;
            contractPayRepository = _contractPayRepository;
            identifierServiceRepository = _identifierServiceRepository;
        }

        
    }
}
