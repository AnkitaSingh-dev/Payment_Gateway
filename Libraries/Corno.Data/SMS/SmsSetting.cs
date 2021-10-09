using Corno.Data.Base;
using System;

namespace Corno.Data.SMS
{
    public class SmsSetting : BaseModel
    {
        public string Occurrence { get; set; }
        public DateTime? DateTime { get; set; }
    }
}
