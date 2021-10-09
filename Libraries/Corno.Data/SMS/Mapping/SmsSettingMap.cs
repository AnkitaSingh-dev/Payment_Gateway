using System.Data.Entity.ModelConfiguration;

namespace Corno.Data.SMS.Mapping
{
    public class SmsSettingMap : EntityTypeConfiguration<SmsSetting>
    {
        public SmsSettingMap()
        {
            // Primary Key
            HasKey(t => t.Id);


            // Table & Column Mappings
            ToTable("SmsSetting");
        }
    }
}