using Corno.Data.Common;
using Corno.Services.Base;
using Corno.Services.Base.Interfaces;
using Corno.Services.Common.Interfaces;

namespace Corno.Services.Common
{
    public class CompanyService : BaseService<Company>, ICompanyService
    {
        public CompanyService(IUnitOfWork unitOfWork, IGenericRepository<Company> repository)
            : base(unitOfWork, repository)
        {
        }
    }
}