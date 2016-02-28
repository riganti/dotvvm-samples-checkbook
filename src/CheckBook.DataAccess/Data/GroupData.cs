
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace CheckBook.DataAccess.Data
{
    public class GroupData
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The Group Name field is required!")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The Currency field is required!")]
        public string Currency { get; set; }

        public decimal TotalSpending { get; set; }

        public int TotalPayments { get; set; }

        public string ImageUrl => "/identicon/group-" + WebUtility.UrlEncode(Id.ToString());
    }
}
