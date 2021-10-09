using Corno.Data.Common;
using Corno.Services.Base;
using Corno.Services.Base.Interfaces;
using Corno.Services.Common.Interfaces;

namespace Corno.Services.Common
{
    public class StateService : MasterService<State>, IStateService
    {
        public StateService(IUnitOfWork unitOfWork, IGenericRepository<State> repository)
            : base(unitOfWork, repository)
        {
        }
    }
}