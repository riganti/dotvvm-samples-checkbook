using System;
using System.ComponentModel.DataAnnotations.Schema;
using CheckBook.DataAccess.Enums;

namespace CheckBook.DataAccess.Model
{
    /// <summary>
    /// Contains information about operation done to a payment
    /// </summary>
    public class PaymentLog : IEntity<int>
    {
        public int Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public int PaymentId { get; set; }

        [ForeignKey(nameof(PaymentId))]
        public virtual Payment Payment { get; set; }

        public int EditorId { get; set; }

        [ForeignKey(nameof(EditorId))]
        public virtual User Editor { get; set; }

        public int UserId { get; set; }

        public LogType LogType { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        public decimal AmountOriginal { get; set; }

        public decimal AmountNew { get; set; }
    }
}
