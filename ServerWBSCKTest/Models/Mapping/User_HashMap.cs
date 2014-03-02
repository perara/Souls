using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ServerWBSCKTest.Models.Mapping
{
    public class User_HashMap : EntityTypeConfiguration<User_Hash>
    {
        public User_HashMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.hash)
                .HasMaxLength(255);

            // Table & Column Mappings
            this.ToTable("User_Hash");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.hash).HasColumnName("hash");
            this.Property(t => t.fk_user_id).HasColumnName("fk_user_id");

            // Relationships
            this.HasOptional(t => t.User)
                .WithMany(t => t.User_Hash)
                .HasForeignKey(d => d.fk_user_id);

        }
    }
}
