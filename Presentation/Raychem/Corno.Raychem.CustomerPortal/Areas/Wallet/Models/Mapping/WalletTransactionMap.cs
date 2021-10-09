using Corno.Raychem.CustomerPortal.Areas.Wallet.Models;
using System.Data.Entity.ModelConfiguration;

namespace Corno.Raychem.CustomerPortal.Areas.cWallet.Models.Mapping
{
    public class WalletTransactionMap : EntityTypeConfiguration<WalletTransaction>
    {
        public WalletTransactionMap()
        {
            // Primary Key
            HasKey(t => t.Id);


            // Table & Column Mappings
            ToTable("WalletTransaction");
        }
    }
}
