using CheckBook.DataAccess.Data.User;
using CheckBook.DataAccess.Model;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace CheckBook.DataAccess.Expressions
{
    public static class GroupExpressions
    {
        public static Expression<Func<UserGroup, UserGroupStatsData>> ToUserGroupStats =>
            ug => new UserGroupStatsData
            {
                GroupId = ug.GroupId,
                GroupName = ug.Group.Name,
                UserBalance = ug.User.Transactions
                    .Where(w => w.Payment.GroupId == ug.GroupId)
                    .Select(s => s.Amount)
                    .DefaultIfEmpty(0)
                    .Sum()
            };
    }
}