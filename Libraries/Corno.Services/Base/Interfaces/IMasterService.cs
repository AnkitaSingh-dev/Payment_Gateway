using Corno.Data.Base;
using System.Collections.Generic;

namespace Corno.Services.Base.Interfaces
{
    public interface IMasterService<TEntity> : IBaseService<TEntity>
        where TEntity : class
    {

        #region -- Methods --
        IList<MasterViewModel> GetIdNameList();
        string GetName(int id);

        #endregion
    }
}