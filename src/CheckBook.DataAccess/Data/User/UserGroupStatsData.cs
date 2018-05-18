using System;
using System.Collections.Generic;
using System.Linq;

namespace CheckBook.DataAccess.Data.User
{
    public class UserGroupStatsData
    {
        public int GroupId { get; set; }

        public string GroupName { get; set; }

        public decimal UserBalance { get; set; }
    }
}