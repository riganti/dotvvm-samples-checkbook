using CheckBook.DataAccess.Model;
using System.Data.Entity;

namespace CheckBook.DataAccess.Context
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Group> Groups { get; set; }

        public DbSet<UserGroup> UserGroups { get; set; }

        public DbSet<Payment> Payments { get; set; }

        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<PaymentLog> PaymentLogs { get; set; }

        public AppDbContext() : base("name=DB")
        {
        }

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