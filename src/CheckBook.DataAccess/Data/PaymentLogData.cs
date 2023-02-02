using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckBook.DataAccess.Enums;
using CheckBook.DataAccess.Model;

namespace CheckBook.DataAccess.Data
{
    public class PaymentLogData
    {
        public DateTime EditDate { get; set; }

        public DateTime PaymentCreatedDate { get; set; }

        public string PaymentDescription { get; set; }

        public string EditorName { get; set; }

        public string UserName { get; set; }

        public LogType LogType { get; set; }

        public string Currency { get; set; }

        public double AmountOriginal { get; set; }

        public double AmountNew { get; set; }
    }
}
