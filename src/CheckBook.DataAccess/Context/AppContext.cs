using System.Data.Entity;
using CheckBook.DataAccess.Model;

namespace CheckBook.DataAccess.Context
{
    public class AppContext : DbContext
    {

        public AppContext() : base("name=DB")
        {
            
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Group> Groups { get; set; }

        public DbSet<UserGroup> UserGroups { get; set; }

        public DbSet<Payment> Payments { get; set; }

        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transaction>()
                        .HasRequired(p => p.User)
                        .WithMany(u => u.Transactions)
                        .HasForeignKey(p => p.UserId)
                        .WillCascadeOnDelete(false);
        }
    }
}
