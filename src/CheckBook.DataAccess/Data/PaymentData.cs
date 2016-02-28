
using System;
using System.ComponentModel.DataAnnotations;

namespace CheckBook.DataAccess.Data
{
    public class PaymentData
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The Date field is required!")]
        public DateTime CreatedDate { get; set; }

        [Required(ErrorMessage = "The Description field is required!")]
        public string Description { get; set; }

        public decimal TotalAmount { get; set; }

        public string Currency { get; set; }

        public int GroupId { get; set; }
    }
}
