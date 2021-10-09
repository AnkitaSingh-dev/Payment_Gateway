using Corno.Data.Base;
using Corno.Raychem.CustomerPortal.Areas.Admin.Models;
using System;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Models
{
    public class UserDMTAgentRT : BaseModel
    {
        public string AgentId { get; set; }
        public string UserName { get; set; }
        public string PermanentAddress { get; set; }
        public int? PinCode { get; set; }
        public string MerchantId { get; set; }
    }
}