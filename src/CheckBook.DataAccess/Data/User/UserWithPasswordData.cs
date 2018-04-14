using System;
using System.Collections.Generic;
using System.Linq;
using CheckBook.DataAccess.Enums;

namespace CheckBook.DataAccess.Data.User
{
    public class UserWithPasswordData
    {

        public int Id { get; set; }

        public UserRole UserRole { get; set; }
        
        public string FirstName { get; set; }

        public string LastName { get; set; }
        
        public string Email { get; set; }
        
        public string PasswordSalt { get; set; }

        public string PasswordHash { get; set; }
        
    }
}
