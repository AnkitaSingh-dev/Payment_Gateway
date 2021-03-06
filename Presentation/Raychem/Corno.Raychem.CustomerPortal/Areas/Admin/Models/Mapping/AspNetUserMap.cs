using System.Data.Entity.ModelConfiguration;

namespace Corno.Raychem.CustomerPortal.Areas.Admin.Models.Mapping
{
    public class AspNetUserMap : EntityTypeConfiguration<AspNetUser>
    {
        public AspNetUserMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            //// Properties
            //this.Property(t => t.Id)
            //    .IsRequired()
            //    .HasMaxLength(128);

            //this.Property(t => t.FirstName)
            //    .IsRequired();

            //this.Property(t => t.LastName)
            //    .IsRequired();

            //this.Property(t => t.Email)
            //    .HasMaxLength(256);

            //this.Property(t => t.UserName)
            //    .IsRequired()
            //    .HasMaxLength(256);

            // Table & Column Mappings
            this.ToTable("AspNetUsers");
            //this.Property(t => t.Id).HasColumnName("Id");
            //this.Property(t => t.FirstName).HasColumnName("FirstName");
            //this.Property(t => t.LastName).HasColumnName("LastName");
            //this.Property(t => t.Email).HasColumnName("Email");
            //this.Property(t => t.EmailConfirmed).HasColumnName("EmailConfirmed");
            //this.Property(t => t.PasswordHash).HasColumnName("PasswordHash");
            //this.Property(t => t.SecurityStamp).HasColumnName("SecurityStamp");
            //this.Property(t => t.PhoneNumber).HasColumnName("PhoneNumber");
            //this.Property(t => t.PhoneNumberConfirmed).HasColumnName("PhoneNumberConfirmed");
            //this.Property(t => t.TwoFactorEnabled).HasColumnName("TwoFactorEnabled");
            //this.Property(t => t.LockoutEndDateUtc).HasColumnName("LockoutEndDateUtc");
            //this.Property(t => t.LockoutEnabled).HasColumnName("LockoutEnabled");
            //this.Property(t => t.AccessFailedCount).HasColumnName("AccessFailedCount");
            //this.Property(t => t.UserName).HasColumnName("UserName");
            //this.Property(t => t.CompanyID).HasColumnName("CompanyID");

            // this.HasOptional(t => t.Company)
            //.WithMany(t => t.AspNetUsers)
            //.HasForeignKey(d => d.CompanyID);
        }
    }
}
