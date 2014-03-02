using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using ServerWBSCKTest.Models.Mapping;

namespace ServerWBSCKTest.Models
{
    public partial class soulsContext : DbContext
    {
        static soulsContext()
        {
            Database.SetInitializer<soulsContext>(null);
        }

        public soulsContext()
            : base("Name=soulsContext")
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<User_Hash> User_Hash { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new UserMap());
            modelBuilder.Configurations.Add(new User_HashMap());
        }
    }
}
