using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckBook.DataAccess.Data
{
    public class MyTransactionData
    {
        public DateTime CreatedDate { get; set; }
        public string Description { get; set; }
        public double MyBalance { get; set; }
        public double MySpending { get; set; }
        public double TotalAmount { get; set; }
        public string Currency { get; set; }
        public int GroupId { get; set; }
        public int PaymentId { get; set; }
        public string GroupName { get; set; }
    }
}
