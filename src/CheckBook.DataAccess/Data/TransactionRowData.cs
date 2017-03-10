using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckBook.DataAccess.Data
{
    public class TransactionRowData : TransactionData
    {
        public int RowId { get; set; }
        public string TbText { get; set; } = "";
        public bool IsUserboxVisible { get; set; } = false;
    }
}
