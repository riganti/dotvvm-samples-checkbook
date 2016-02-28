using System.Data.Entity.Migrations;
using System.Linq;
using CheckBook.DataAccess.Enums;
using CheckBook.DataAccess.Model;
using CheckBook.DataAccess.Security;

namespace CheckBook.DataAccess.Migrations
{
    public sealed class Configuration : DbMigrationsConfiguration<DataAccess.Context.AppContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(DataAccess.Context.AppContext context)
        {
            if (!context.Users.Any())
            {
                // create sample groups
                var prague = new Group { Name = "Lunches - Riganti Office", Currency = "CZK" };
                context.Groups.Add(prague);

                var brno = new Group { Name = "Brno - Other Activities", Currency = "EUR" };
                context.Groups.Add(brno);


                // create sample users
                var password1 = PasswordHelper.CreateHash("Pa$$w0rd");
                var user1 = new User
                {
                    FirstName = "John",
                    LastName = "Newman",
                    Email = "newman@test.com",
                    UserRole = UserRole.User,
                    PasswordHash = password1.PasswordHash,
                    PasswordSalt = password1.PasswordSalt
                };
                context.Users.Add(user1);
                context.UserGroups.Add(new UserGroup() { User = user1, Group = prague });
            
                var password2 = PasswordHelper.CreateHash("Pa$$w0rd");
                var user2 = new User
                {
                    FirstName = "John",
                    LastName = "Smith",
                    Email = "smith@test.com",
                    UserRole = UserRole.Admin,
                    PasswordHash = password2.PasswordHash,
                    PasswordSalt = password2.PasswordSalt
                };

                context.Users.Add(user2);
                context.UserGroups.Add(new UserGroup() { User = user2, Group = prague });
                context.UserGroups.Add(new UserGroup() { User = user2, Group = brno });
            
                var password3 = PasswordHelper.CreateHash("Pa$$w0rd");
                var user3 = new User
                {
                    FirstName = "David",
                    LastName = "Anderson",
                    Email = "anderson@test.com",
                    UserRole = UserRole.User,
                    PasswordHash = password3.PasswordHash,
                    PasswordSalt = password3.PasswordSalt
                };

                context.Users.Add(user3);
                context.UserGroups.Add(new UserGroup() { User = user3, Group = prague });
                context.UserGroups.Add(new UserGroup() { User = user3, Group = brno });
            }
        }
    }
}
