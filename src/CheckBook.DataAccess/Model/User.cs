using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CheckBook.DataAccess.Enums;

namespace CheckBook.DataAccess.Model
{

    /// <summary>
    /// A user who can login into the application.
    /// </summary>
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [Required]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(100)]
        public string PasswordSalt { get; set; }

        [Required]
        [StringLength(100)]
        public string PasswordHash { get; set; }

        [StringLength(200)]
        public string ImageUrl { get; set; }

        public UserRole UserRole { get; set; }

        public virtual ICollection<UserGroup> UserGroups { get; private set; }

        public virtual ICollection<Transaction> Transactions { get; private set; }
        

        public User()
        {
            UserGroups = new List<UserGroup>();
            Transactions = new List<Transaction>();
        }
    }
}
