using System.Data.Entity.ModelConfiguration;

namespace Corno.Raychem.CustomerPortal.Areas.Admin.Models.Mapping
{
    public class AspNetRoleMap : EntityTypeConfiguration<AspNetRole>
    {
        public AspNetRoleMap()
        {
            // Primary Key
            //this.HasKey(t => t.Id);

            // Properties
           

            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(256);

            // Table & Column Mappings
            this.ToTable("AspNetRoles");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Name).HasColumnName("Name");

            // Relationships
            this.HasMany(t => t.AspNetUsers)
                .WithMany(t => t.AspNetRoles)
                .Map(m =>
                    {
                        m.ToTable("AspNetUserRoles");
                        m.MapLeftKey("RoleId");
                        m.MapRightKey("UserId");
                    });


        }
    }
}
