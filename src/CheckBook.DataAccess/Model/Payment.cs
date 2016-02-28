using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CheckBook.DataAccess.Model
{

    /// <summary>
    /// A group of transactions which belong together (e.g. one person paid for a cinema for four others).
    /// </summary>
    public class Payment
    {
        public int Id { get; set; }

        public DateTime CreatedDate { get; set; }

        [Required]
        [StringLength(100)]
        public string Description { get; set; }

        public int GroupId { get; set; }

        public virtual Group Group { get; set; }

        public virtual ICollection<Transaction> Transactions { get; private set; }


        public Payment()
        {
            Transactions = new List<Transaction>();
        }
    }
}
