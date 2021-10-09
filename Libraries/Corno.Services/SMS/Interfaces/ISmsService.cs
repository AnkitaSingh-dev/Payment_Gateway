using System;
using Corno.Services.Base.Interfaces;

namespace Corno.Services.SMS.Interfaces
{
    public interface ISmsService<TEntity> : IBaseService<TEntity>
        where TEntity : class
    {
        #region -- Properties --
        string UserName { get; set; }
        string Password { get; set; }
        string SenderId { get; set; }
        string Channel { get; set; }
        string Route { get; set; }
        #endregion

        #region -- Methods --
        string SendSms(string phoneNo, string smsBody);
        void SaveSmsLog(string phoneNo, string smsBody, string smsResult,
            DateTime transactionDate, string transactionType, int departmentId);
        #endregion
    }
}