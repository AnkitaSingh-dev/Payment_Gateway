using Corno.Data.SMS;
using Corno.Services.Base;
using Corno.Services.Base.Interfaces;
using Corno.Services.SMS.Interfaces;

namespace Corno.Services.SMS
{
    public class SmsSettingsService : BaseService<SmsSetting>, ISmsSettingsService
    {
        #region -- Constructors --

        public SmsSettingsService(IUnitOfWork unitOfWork, IGenericRepository<SmsSetting> smsSettingsRepository)
            : base(unitOfWork, smsSettingsRepository)
        {
        }

        #endregion

    }
}