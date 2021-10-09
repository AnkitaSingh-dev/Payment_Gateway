using System.Data.Entity;
using Corno.Data.Context;
using Corno.Raychem.CustomerPortal.Areas.Admin.Models;
using Corno.Raychem.CustomerPortal.Areas.Admin.Models.Mapping;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Models;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Models.Air;

namespace Corno.Raychem.CustomerPortal.Contexts
{
    public class CustomerPortalContext : CornoContext
    {
        #region -- Constructors --

        public CustomerPortalContext(string connectionString)
            : base(connectionString)
        {
        }

        public CustomerPortalContext() : base(@"Name=CornoContext")
        {
        }

        #endregion

        #region -- Data Members --

        // Admin
        public DbSet<AspNetRole> AspNetRoles { get; set; }
        public DbSet<AspNetUser> AspNetUsers { get; set; }
        public DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

        #endregion

        #region -- Mappings --

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Admin
            modelBuilder.Configurations.Add(new AspNetRoleMap());
            modelBuilder.Configurations.Add(new AspNetUserClaimMap());
            modelBuilder.Configurations.Add(new AspNetUserLoginMap());
            modelBuilder.Configurations.Add(new AspNetUserMap());

            // Wallet
            modelBuilder.Entity<WalletTransaction>().ToTable("WalletTransaction");
            modelBuilder.Entity<CyberPlatUrl>().ToTable("CyberPlatUrl");
            modelBuilder.Entity<CyberPlatError>().ToTable("CyberPlatError");
            modelBuilder.Entity<Airport>().ToTable("Airport");
            modelBuilder.Entity<BusCity>().ToTable("BusCity");
            modelBuilder.Entity<HotelCity>().ToTable("HotelCity");
            modelBuilder.Entity<Mpos>().ToTable("Mpos");
            modelBuilder.Entity<MasterTransaction>().ToTable("MasterCommission");

           // modelBuilder.Entity<MasterCommission>().ToTable("MasterCommission");

            modelBuilder.Entity<Commission>().ToTable("Commission");
            modelBuilder.Entity<Society>().ToTable("MasterSociety");
            modelBuilder.Entity<SocietyUserRT>().ToTable("SocietyUsersRT");
            modelBuilder.Entity<UserKYCModel>().ToTable("UserKYC");
            modelBuilder.Entity<UserDMTAgentRT>().ToTable("UserDMTAgentRT");
            modelBuilder.Entity<StateCityUT>().ToTable("StateCityUT");
            modelBuilder.Entity<JugadUserSafexUserRT>().ToTable("JugadUserSafexUserRT");
            modelBuilder.Entity<PrePaidCardKitNo>().ToTable("PrePaidCardKitNo");
            modelBuilder.Entity<SafexWalletTransaction>().ToTable("SafexWalletTransaction");
            modelBuilder.Entity<Order_Status_Result>().ToTable("PGTransactions");
            modelBuilder.Entity<OperatorSeriesRT>().ToTable("OperatorSeriesRT");
            modelBuilder.Entity<PrepaidPlans>().ToTable("PrepaidPlans");
        }

        #endregion
    }
}