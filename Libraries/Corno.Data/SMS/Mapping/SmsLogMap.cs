using System.Data.Entity.ModelConfiguration;

namespace Corno.Data.SMS.Mapping
{
    public class SmsLogMap : EntityTypeConfiguration<SmsLog>
    {
        public SmsLogMap()
        {
            // Primary Key
            HasKey(t => t.Id);


            // Table & Column Mappings
            ToTable("SmsLog");
        }
    }
}