using System.ComponentModel.DataAnnotations.Schema;
using CheckBook.DataAccess.Enums;

namespace CheckBook.DataAccess.Model
{
    public class Transaction
    {
        public int Id { get; set; }
        
        public decimal Amount { get; set; }
        
        public TransactionType Type { get; set; }

        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        public int PaymentId { get; set; }

        public virtual Payment Payment { get; set; }
    }
}
