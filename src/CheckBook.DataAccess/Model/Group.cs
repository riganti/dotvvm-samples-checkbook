using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CheckBook.DataAccess.Model
{

    /// <summary>
    /// A group of users which share their payments.
    /// </summary>
    public class Group
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(10)]
        public string Currency { get; set; }

        public virtual ICollection<Payment> Payments { get; private set; }

        public virtual ICollection<UserGroup> UserGroups { get; private set; }


        public Group()
        {
            Payments = new List<Payment>();
            UserGroups = new List<UserGroup>();
        }
    }
}
