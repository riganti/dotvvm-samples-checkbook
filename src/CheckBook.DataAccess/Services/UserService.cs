using CheckBook.DataAccess.Context;
using CheckBook.DataAccess.Data.User;
using CheckBook.DataAccess.Expressions;
using CheckBook.DataAccess.Model;
using CheckBook.DataAccess.Security;
using DotVVM.Framework.Controls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CheckBook.DataAccess.Services
{
    public static class UserService
    {
        /// <summary>
        /// Gets the user with specified e-mail address. 
        /// </summary>
        public static UserWithPasswordData GetUserWithPassword(string email)
        {
            using (var db = new AppDbContext())
            {
                email = email.Trim().ToLower();

                return db.Users
                    .Select(UserExpressions.ToUserWithPasswordData)
                    .FirstOrDefault(x => x.Email == email);
            }
        }

        /// <summary>
        /// Gets the user profile. 
        /// </summary>
        public static UserInfoData GetUserInfo(int id)
        {
            using (var db = new AppDbContext())
            {
                return db.Users
                    .Select(UserExpressions.ToUserInfoData)
                    .First(x => x.Id == id);
            }
        }

        public static UserStatsData GetUserStats(int id)
        {
            using (var db = new AppDbContext())
            {
                var user = db.Users.Find(id);
                return UserExpressions.ToUserStatsData.Compile()(user);
            }
        }

        /// <summary>
        /// Gets the user basic info. 
        /// </summary>
        public static List<UserBasicInfoData> GetUserBasicInfoList(int groupId)
        {
            using (var db = new AppDbContext())
            {
                return db.Users
                    .Where(u => u.UserGroups.Any(g => g.GroupId == groupId))
                    .Select(UserExpressions.ToUserBasicInfoData)
                    .OrderBy(x => x.Name)
                    .ToList();
            }
        }

        /// <summary>
        /// Gets the user profile. 
        /// </summary>
        public static void LoadUserInfos(GridViewDataSet<UserInfoData> dataSet)
        {
            using (var db = new AppDbContext())
            {
                var users = db.Users
                    .Select(UserExpressions.ToUserInfoData);

                dataSet.LoadFromQueryable(users);
            }
        }

        /// <summary>
        /// Searches for the users. 
        /// </summary>
        public static List<UserInfoData> SearchUsers(string searchText)
        {
            using (var db = new AppDbContext())
            {
                IQueryable<User> users = db.Users;
                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    users = users.Where(u => (u.FirstName + " " + u.LastName + " " + u.Email).Contains(searchText));
                }

                return users
                    .OrderBy(u => u.LastName)
                    .Select(UserExpressions.ToUserInfoData)
                    .Take(8)
                    .ToList();
            }
        }

        /// <summary>
        /// Gets the users in the specified group. 
        /// </summary>
        public static List<UserInfoData> GetGroupUsers(int groupId)
        {
            using (var db = new AppDbContext())
            {
                return db.Users
                    .Where(u => u.UserGroups.Any(g => g.GroupId == groupId))
                    .OrderBy(u => u.LastName)
                    .Select(UserExpressions.ToUserInfoData)
                    .ToList();
            }
        }

        /// <summary>
        /// Updates the user data (from the Settings page). 
        /// </summary>
        public static void UpdateUserInfo(UserInfoData user, int userId)
        {
            using (var db = new AppDbContext())
            {
                var entity = db.Users.Find(userId);

                UpdateUserInfoCore(user, entity, db);

                db.SaveChanges();
            }
        }

        /// <summary>
        /// Creates the or update user information (from the Manager page). 
        /// </summary>
        public static void CreateOrUpdateUserInfo(UserInfoData user)
        {
            using (var db = new AppDbContext())
            {
                var entity = db.Users.Find(user.Id);
                if (entity == null)
                {
                    if (string.IsNullOrWhiteSpace(user.Password))
                    {
                        throw new Exception("The Password is required!");
                    }

                    entity = new User();
                    db.Users.Add(entity);
                }

                UpdateUserInfoCore(user, entity, db);
                entity.UserRole = user.UserRole;

                db.SaveChanges();
            }
        }

        /// <summary>
        /// Deletes the user. 
        /// </summary>
        public static void DeleteUser(int id)
        {
            using (var db = new AppDbContext())
            {
                var user = db.Users.Find(id);
                if (user.Transactions.Any())
                {
                    throw new Exception("The user cannot be removed because he is involved in one or more transactions!");
                }

                db.Users.Remove(user);
                db.SaveChanges();
            }
        }

        private static void UpdateUserInfoCore(UserInfoData user, User entity, AppDbContext db)
        {
            // update first and last name
            entity.FirstName = user.FirstName;
            entity.LastName = user.LastName;
            entity.ImageUrl = user.ImageUrl;

            // update the password
            if (!string.IsNullOrWhiteSpace(user.Password))
            {
                var passwordData = PasswordHelper.CreateHash(user.Password);
                entity.PasswordSalt = passwordData.PasswordSalt;
                entity.PasswordHash = passwordData.PasswordHash;
            }

            // update the e-mail and check e-mail uniqueness
            if (db.Users.Any(u => u.Id != user.Id && u.Email == user.Email))
            {
                throw new Exception($"The user with e-mail address '{user.Email}' already exists!");
            }
            entity.Email = user.Email;
        }
    }
}