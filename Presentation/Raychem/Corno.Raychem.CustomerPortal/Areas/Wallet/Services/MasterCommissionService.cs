using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Models;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Services.Interfaces;
using Corno.Services.Base.Interfaces;
using Corno.Services.Base;
//using Corno.Services.Bootstrapper;
//using Corno.Raychem.CustomerPortal.Areas.Admin.Models;
//using Corno.Raychem.CustomerPortal.Areas.Wallet.Constants;



namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Services
{
    public class MasterCommissionService : BaseService<MasterCommission>, IMasterCommissionService
    {
        //#region -- Constructors --
        //public MasterCommissionService(IUnitOfWork unitOfWork, IGenericRepository<MasterCommission> masterCommissionRepository)
        //    : base(unitOfWork, masterCommissionRepository)
        //{
        //}
        //#endregion

        #region -- Constructors --
        public MasterCommissionService(IUnitOfWork unitOfWork, IGenericRepository<MasterCommission> masterTransactionRepository)
            : base(unitOfWork, masterTransactionRepository)
        {
        }
        #endregion

        //#region -- Data Member --
        //private readonly IMasterCommissionService _masterCommissionService;
        //#endregion


        //void IMasterCommissionService.GetCommission()
        //{           
        //    var masterCommissionService = (IMasterCommissionService)Bootstrapper.GetService(typeof(IMasterCommissionService));
        //    string service = "DTH", oper = "Airtel";
        //    var masterCommission = _masterCommissionService.Get(a => a.Service == service && a.Operator == oper).FirstOrDefault();           
        //}
    }
}