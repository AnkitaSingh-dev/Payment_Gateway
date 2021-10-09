
using Corno.Globals.Constants;
using Corno.Raychem.CustomerPortal.Areas.Admin.Models;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Models;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Services.Interfaces;
using Corno.Services.Base;
using Corno.Services.Base.Interfaces;
using System.Web;
using static Corno.Raychem.CustomerPortal.Areas.Wallet.Models.PrePaidCardModel;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Services
{
    public class SafexWalletTransactionService : BaseService<SafexWalletTransaction>, ISafexWalletTransactionService
    {
        #region -- Constructors --
        public SafexWalletTransactionService(IUnitOfWork unitOfWork, IGenericRepository<SafexWalletTransaction> SafexWalletTransactionRepository)
            : base(unitOfWork, SafexWalletTransactionRepository)
        {
        }
        #endregion
        public void AddSafexWalletTransaction(SafexWalletTransaction model)
        {
            Add(model);
            Save();
        }
    }
}