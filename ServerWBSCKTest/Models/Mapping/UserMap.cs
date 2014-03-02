using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ServerWBSCKTest.Models.Mapping
{
    public class UserMap : EntityTypeConfiguration<User>
    {
        public UserMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.name)
                .IsRequired()
                .HasMaxLength(255);

            this.Property(t => t.password)
                .IsRequired()
                .HasMaxLength(255);

            this.Property(t => t.timestamp)
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            this.ToTable("User");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.name).HasColumnName("name");
            this.Property(t => t.password).HasColumnName("password");
            this.Property(t => t.rank).HasColumnName("rank");
            this.Property(t => t.timestamp).HasColumnName("timestamp");
        }
    }
}
