using Corno.Data.Base;
using System;

namespace Corno.Data.SMS
{
    public class SmsLog : BaseModel
    {
        public DateTime? SendDate { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string TransactionType { get; set; }
        public int? DepartmentId { get; set; }
        public string FromNo { get; set; }
        public string ToNo { get; set; }
        public string Message { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string SenderId { get; set; }
        public string Priority { get; set; }
        public string SmsType { get; set; }
        public string SmsResult { get; set; }
    }
}