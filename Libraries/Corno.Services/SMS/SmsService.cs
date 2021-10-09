using Corno.Services.Base;
using Corno.Services.Base.Interfaces;
using Corno.Services.SMS.Interfaces;
using System;

namespace Corno.Services.SMS
{
    public class SmsService<TEntity> : BaseService<TEntity>, ISmsService<TEntity>
        where TEntity : class
    {
        #region -- Constructors --
        public SmsService(IUnitOfWork unitOfWork, IGenericRepository<TEntity> entityRepository)
            : base(unitOfWork, entityRepository)
        {
        }

        //public SmsService(string userName, string password, string senderId)
        //{
        //    UserName = userName;
        //    Password = password;
        //    SenderId = senderId;
        //}
        #endregion

        #region -- Properties --
        public string UserName { get; set; }
        public string Password { get; set; }
        public string SenderId { get; set; }
        public string Channel { get; set; }
        public string Route { get; set; }
        #endregion

        #region -- Methods --
        public virtual string SendSms(string phoneNo, string smsBody)
        {
            throw new NotImplementedException();
        }

        public virtual void SaveSmsLog(string phoneNo, string smsBody, string smsResult,
            DateTime transactionDate, string transactionType, int departmentId)
        {
            throw new NotImplementedException();
        }
    }
    #endregion
}