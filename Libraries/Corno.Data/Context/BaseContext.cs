using System.Data.Entity;

namespace Corno.Data.Context
{
    public class BaseContext : DbContext
    {
        public BaseContext(string connectionString)
            : base(connectionString)
        {
            Database.SetInitializer<BaseContext>(null);
        }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<CornoContext>(null);

            // Call base class function
            base.OnModelCreating(modelBuilder);
        }
    }
}