using Corno.Raychem.CustomerPortal.Areas.Admin.Models;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Models;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Services.Interfaces
{
    public interface ICommissionApiServices
    {
        #region -- Methods --
        double GetConvinienceCharges(string service, string oper, double amount);

        double? GetCommission(string service, string oper, double amount,ApplicationUser user);
        double GetGst(double amount);
        double GetCommissionDmt(double amount, ApplicationUser user);

        #endregion
    }
}