using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CheckBook.DataAccess.Context;
using CheckBook.DataAccess.Data;
using CheckBook.DataAccess.Model;
using CheckBook.DataAccess.Security;
using DotVVM.Framework.Controls;

namespace CheckBook.DataAccess.Services
{
    public class UserService
    {
        private readonly AppDbContext db;

        public UserService(AppDbContext db)
        {
            this.db = db;
        }

        /// <summary>
        /// Gets the user with specified e-mail address.
        /// </summary>
        public UserWithPasswordData GetUserWithPassword(string email)
        {
            email = email.Trim().ToLower();

            return db.Users
                .Select(ToUserWithPasswordData)
                .FirstOrDefault(x => x.Email == email);
        }

        /// <summary>
        /// Gets the user profile.
        /// </summary>
        public UserInfoData GetUserInfo(int id)
        {
            return db.Users
                .Select(ToUserInfoData)
                .First(x => x.Id == id);
        }


        /// <summary>
        /// Gets the user basic info.
        /// </summary>
        public List<UserBasicInfoData> GetUserBasicInfoList(int groupId)
        {
            return db.Users
                .Where(u => u.UserGroups.Any(g => g.GroupId == groupId))
                .Select(ToUserBasicInfoData)
                .OrderBy(x => x.Name)
                .ToList();
        }

        /// <summary>
        /// Gets the user profile.
        /// </summary>
        public void LoadUserInfos(GridViewDataSet<UserInfoData> dataSet)
        {
            var users = db.Users
                .Select(ToUserInfoData);

            dataSet.LoadFromQueryable(users);
        }

        /// <summary>
        /// Searches for the users.
        /// </summary>
        public List<UserInfoData> SearchUsers(string searchText)
        {
            IQueryable<User> users = db.Users;
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                users = users.Where(u => (u.FirstName + " " + u.LastName + " " + u.Email).Contains(searchText));
            }

            return users
                .OrderBy(u => u.LastName)
                .Select(ToUserInfoData)
                .Take(8)
                .ToList();
        }

        /// <summary>
        /// Gets the users in the specified group.
        /// </summary>
        public List<UserInfoData> GetGroupUsers(int groupId)
        {
            return db.Users
                .Where(u => u.UserGroups.Any(g => g.GroupId == groupId))
                .OrderBy(u => u.LastName)
                .Select(ToUserInfoData)
                .ToList();
        }

        /// <summary>
        /// Updates the user data (from the Settings page).
        /// </summary>
        public void UpdateUserInfo(UserInfoData user, int userId)
        {
            var entity = db.Users.Find(userId);

            UpdateUserInfoCore(user, entity, db);

            db.SaveChanges();
        }

        /// <summary>
        /// Creates the or update user information (from the Manager page).
        /// </summary>
        public void CreateOrUpdateUserInfo(UserInfoData user)
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

        private void UpdateUserInfoCore(UserInfoData user, User entity, AppDbContext db)
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


        /// <summary>
        /// Deletes the user.
        /// </summary>
        public void DeleteUser(int id)
        {
            var user = db.Users.Find(id);
            if (user.Transactions.Any())
            {
                throw new Exception("The user cannot be removed because he is involved in one or more transactions!");
            }

            db.Users.Remove(user);
            db.SaveChanges();
        }

        /// <summary>
        /// Converts the User entity into the UserWithPasswordData object
        /// </summary>
        public static Expression<Func<User, UserWithPasswordData>> ToUserWithPasswordData
        {
            get
            {
                return u => new UserWithPasswordData()
                {
                    Id = u.Id,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    PasswordHash = u.PasswordHash,
                    PasswordSalt = u.PasswordSalt,
                    UserRole = u.UserRole
                };
            }
        }

        /// <summary>
        /// Converts User entity into UserInfoData
        /// </summary>
        public static Expression<Func<User, UserInfoData>> ToUserInfoData
        {
            get
            {
                return u => new UserInfoData()
                {
                    Id = u.Id,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    ImageUrl = u.ImageUrl,
                    UserRole = u.UserRole,
                    Name = u.FirstName + " " + u.LastName
                };
            }
        }


        /// <summary>
        /// Converts User entity into UserInfoData
        /// </summary>
        public static Expression<Func<User, UserBasicInfoData>> ToUserBasicInfoData
        {
            get
            {
                return u => new UserBasicInfoData()
                {
                    Id = u.Id,
                    Name = u.FirstName + " " + u.LastName,
                    ImageUrl = u.ImageUrl
                };
            }
        }
    }
}
