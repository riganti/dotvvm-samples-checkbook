using CheckBook.DataAccess.Data.User;
using CheckBook.DataAccess.Model;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace CheckBook.DataAccess.Expressions
{
    public static class UserExpressions
    {
        public static Expression<Func<User, UserStatsData>> ToUserStatsData =>
            u => new UserStatsData
            {
                TotalBalance = u.Transactions.Sum(t => (decimal?)t.Amount) ?? 0,
                Groups = u.UserGroups.AsQueryable().Select(GroupExpressions.ToUserGroupStats)
            };

        /// <summary>
        /// Converts the User entity into the UserWithPasswordData object 
        /// </summary>
        public static Expression<Func<User, UserWithPasswordData>> ToUserWithPasswordData =>
            u => new UserWithPasswordData()
            {
                Id = u.Id,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                PasswordHash = u.PasswordHash,
                PasswordSalt = u.PasswordSalt,
                UserRole = u.UserRole
            };

        /// <summary>
        /// Converts User entity into UserInfoData 
        /// </summary>
        public static Expression<Func<User, UserInfoData>> ToUserInfoData =>
            u => new UserInfoData()
            {
                Id = u.Id,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                ImageUrl = u.ImageUrl,
                UserRole = u.UserRole,
                Name = u.FirstName + " " + u.LastName
            };

        /// <summary>
        /// Converts User entity into UserInfoData 
        /// </summary>
        public static Expression<Func<User, UserBasicInfoData>> ToUserBasicInfoData =>
            u => new UserBasicInfoData()
            {
                Id = u.Id,
                Name = u.FirstName + " " + u.LastName,
                ImageUrl = u.ImageUrl
            };
    }
}