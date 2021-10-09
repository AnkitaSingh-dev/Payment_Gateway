using Corno.Data.Common;
using Corno.Data.SMS;
using System.Data.Entity;

namespace Corno.Data.Context
{
    public class CornoContext : BaseContext
    {
        public CornoContext(string connectionString)
            : base(connectionString)
        {
            Database.SetInitializer<CornoContext>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<CornoContext>(null);

            // Call base class function
            base.OnModelCreating(modelBuilder);

            // Masters
            modelBuilder.Entity<Company>().ToTable("Company");
            modelBuilder.Entity<Location>().ToTable("Location");
            modelBuilder.Entity<City>().ToTable("City");
            modelBuilder.Entity<State>().ToTable("State");
            modelBuilder.Entity<Country>().ToTable("Country");
            
            // SMS
            modelBuilder.Entity<SmsLog>().ToTable("SmsLog");
            modelBuilder.Entity<SmsSetting>().ToTable("SmsSetting");
        }
    }
}