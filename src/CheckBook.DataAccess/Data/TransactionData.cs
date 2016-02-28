using System;
using CheckBook.DataAccess.Enums;

namespace CheckBook.DataAccess.Data
{
    public class TransactionData : IAvatarData
    {
        public int? Id { get; set; }

        public int UserId { get; set; }

        public string ImageUrl { get; set; }

        public string Name { get; set; }

        public decimal? Amount { get; set; }
        
    }
}
