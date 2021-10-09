
using Corno.Globals;
using Corno.Globals.Constants;
using Corno.Raychem.CustomerPortal.Areas.Admin.Controllers;
using Corno.Raychem.CustomerPortal.Areas.Admin.Models;
using Corno.Raychem.CustomerPortal.Areas.Admin.Services;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Services;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Services.Interfaces;
using Corno.Raychem.CustomerPortal.Contexts;
using Corno.Services.Bootstrapper;
using Microsoft.Practices.Unity;

namespace Corno.Raychem.CustomerPortal.Helpers
{
    public class BootstrapperLocal
    {
        public static void BootStrapperInitialize()
        {
            if (null != GlobalVariables.Container)
                return;

            GlobalVariables.Container =
                Bootstrapper.Initialise(new CustomerPortalContext("Name = " + FieldConstants.CornoContext));

            GlobalVariables.Container.RegisterType<AccountController>(new InjectionConstructor());

            // Admin
            Bootstrapper.RegisterType(typeof(IAspNetRoleService), typeof(AspNetRoleService));
            Bootstrapper.RegisterType(typeof(IAspNetUserService), typeof(AspNetUserService));
            Bootstrapper.RegisterType(typeof(IdentityManager), typeof(IdentityManager));

            Bootstrapper.RegisterType(typeof(IOperatorSeriesRTService), typeof(OperatorSeriesRTService));
            Bootstrapper.RegisterType(typeof(IPrepaidPlansService), typeof(PrepaidPlansService));

            // Wallet
            Bootstrapper.RegisterType(typeof(IWalletTransactionService), typeof(WalletTransactionService));
            Bootstrapper.RegisterType(typeof(ICyberPlatApiService), typeof(CyberPlatApiService));
            Bootstrapper.RegisterType(typeof(ICyberPlatUrlService), typeof(CyberPlatUrlService));
            Bootstrapper.RegisterType(typeof(ICyberPlatErrorService), typeof(CyberPlatErrorService));
            Bootstrapper.RegisterType(typeof(ICyberPlatDmtService), typeof(CyberPlatDmtService));
            Bootstrapper.RegisterType(typeof(IAirportService), typeof(AirportService));
            Bootstrapper.RegisterType(typeof(IBusCityService), typeof(BusCityService));
            Bootstrapper.RegisterType(typeof(IHotelCityService), typeof(HotelCityService));
            Bootstrapper.RegisterType(typeof(ILogService), typeof(LogService));
            Bootstrapper.RegisterType(typeof(IWalletService), typeof(WalletService));
            Bootstrapper.RegisterType(typeof(IWalletBaseService), typeof(WalletBaseService));

            Bootstrapper.RegisterType(typeof(ICyberPlatService), typeof(CyberPlatService));
            Bootstrapper.RegisterType(typeof(IMposService), typeof(MposService));

            Bootstrapper.RegisterType(typeof(ICommissionApiServices), typeof(CommissionApiServices));
            Bootstrapper.RegisterType(typeof(ICommissionService), typeof(CommissionService));
            Bootstrapper.RegisterType(typeof(IMasterTransactionService), typeof(MasterTransactionService));
            //Bootstrapper.RegisterType(typeof(IMasterCommissionService), typeof(MasterCommissionService));

            Bootstrapper.RegisterType(typeof(ISocietyService), typeof(SocietyService));
            Bootstrapper.RegisterType(typeof(IUserSocietyRT), typeof(UserSocietyRT));

            Bootstrapper.RegisterType(typeof(IUserKYCService), typeof(UserKYCService));
            Bootstrapper.RegisterType(typeof(IUserDMTAgentRTService), typeof(UserDMTAgentRTService));
            Bootstrapper.RegisterType(typeof(IStateCityUTService), typeof(StateCityUTService));

            Bootstrapper.RegisterType(typeof(IPrePaidCardService), typeof(PrePaidCardService));
            Bootstrapper.RegisterType(typeof(IJugadUserSafexUserService), typeof(JugadUserSafexUserService));
            Bootstrapper.RegisterType(typeof(IPrePaidCardKitService), typeof(PrePaidCardKitService));
            Bootstrapper.RegisterType(typeof(ISafexWalletTransactionService), typeof(SafexWalletTransactionService));

            Bootstrapper.RegisterType(typeof(IPGTransactionService), typeof(PGTransactionService));
            Bootstrapper.RegisterType(typeof(IFederalPrePaidCardService), typeof(FederalPrePaidCardService));
        }
    }
}