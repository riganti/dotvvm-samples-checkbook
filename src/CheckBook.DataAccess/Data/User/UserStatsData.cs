using System.Collections.Generic;

namespace CheckBook.DataAccess.Data.User
{
    public class UserStatsData
    {
        public decimal TotalBalance { get; set; }

        public IEnumerable<UserGroupStatsData> Groups { get; set; }
    }
}