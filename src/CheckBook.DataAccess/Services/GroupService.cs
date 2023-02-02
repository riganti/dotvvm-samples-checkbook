using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using CheckBook.DataAccess.Context;
using CheckBook.DataAccess.Data;
using CheckBook.DataAccess.Model;
using DotVVM.Framework.Controls;

namespace CheckBook.DataAccess.Services
{
    public class GroupService
    {
        private readonly AppDbContext db;

        public GroupService(AppDbContext db)
        {
            this.db = db;
        }

        /// <summary>
        /// Loads all groups in the specified dataset.
        /// </summary>
        public void LoadGroups(GridViewDataSet<GroupData> dataSet)
        {
            var groups = db.Groups
                .Select(ToGroupData);
            dataSet.LoadFromQueryable(groups);
        }

        /// <summary>
        /// Gets all groups for the specified user.
        /// </summary>
        public List<GroupData> GetGroupsByUser(int userId)
        {
            return db.Groups
                .Where(g => g.UserGroups.Any(ug => ug.UserId == userId))
                .OrderBy(x => x.Name)
                .Select(ToGroupData).ToList();
        }

        /// <summary>
        /// Gets the group by ID with permission check.
        /// </summary>
        public GroupData GetGroup(int groupId, int userId)
        {
            return GetGroupsByUser(userId).Single(g => g.Id == groupId);
        }

        /// <summary>
        /// Gets the group by ID with permission check.
        /// </summary>
        public GroupData GetGroup(int groupId)
        {
            return db.Groups
                .Select(ToGroupData)
                .Single(g => g.Id == groupId);
        }

        /// <summary>
        /// Creates or updates a specified group.
        /// </summary>
        public void CreateOrUpdateGroup(GroupData group, List<UserInfoData> groupUsers)
        {
            // get or create the group
            var entity = db.Groups.Include(u => u.UserGroups).SingleOrDefault(g => g.Id == group.Id);
            if (entity == null)
            {
                entity = new Group();
                db.Groups.Add(entity);
            }

            // update group properties
            entity.Currency = group.Currency;
            entity.Name = group.Name;

            // add users who are not in the group and should be there
            var usersToAdd = groupUsers.Where(u => !entity.UserGroups.Any(ug => ug.UserId == u.Id)).ToList();
            foreach (var user in usersToAdd)
            {
                entity.UserGroups.Add(new UserGroup() { UserId = user.Id });
            }

            // remove users who are in the group and should not be there
            var usersToRemove = entity.UserGroups.Where(ug => !groupUsers.Any(u => u.Id == ug.UserId)).ToList();
            foreach (var user in usersToRemove)
            {
                // if the user has non-zero balance, he cannot be removed
                var balance = user.User.Transactions.Where(t => t.Payment.GroupId == group.Id).Sum(t => (double?)t.Amount) ?? 0;
                if (balance != 0)
                {
                    throw new Exception($"Cannot remove the user {user.User.FirstName} {user.User.LastName} from the group because he has non-zero balance. You have to settle first!");
                }

                db.UserGroups.Remove(user);
            }

            db.SaveChanges();
        }

        /// <summary>
        /// Deletes the group.
        /// </summary>
        public void DeleteGroup(int id)
        {
            var group = db.Groups.Find(id);
            db.Groups.Remove(group);
            db.SaveChanges();
        }

        /// <summary>
        /// Gets a list of members in a group.
        /// </summary>
        public List<GroupMemberData> GetGroupMembers(int groupId)
        {
            var toGroupMemberData = GetToGroupMemberData(groupId);

            return db.Users
                .Where(u => u.UserGroups.Any(g => g.GroupId == groupId))
                .Select(toGroupMemberData)
                .OrderBy(u => u.Amount)
                .ToList();
        }

        /// <summary>
        /// Converts Group entity into GroupData
        /// </summary>
        public static Expression<Func<Group, GroupData>> ToGroupData
        {
            get
            {
                return g => new GroupData()
                {
                    Id = g.Id,
                    Name = g.Name,
                    Currency = g.Currency,
                    TotalPayments = g.Payments.Count(),
                    TotalSpending = g.Payments.Sum(pg => pg.Transactions.Where(p => p.Amount > 0).Sum(p => (double?)p.Amount)) ?? 0,
                    // We need to cast to (double?) because the result of the expression is NULL when there are no groups
                    // and null is not assignable in the property of double
                };
            }
        }

        private static Expression<Func<User, GroupMemberData>> GetToGroupMemberData(int groupId)
        {
            return u => new GroupMemberData()
            {
                UserId = u.Id,
                Name = u.FirstName + " " + u.LastName,
                ImageUrl = u.ImageUrl,
                Amount = u.Transactions.Where(t => t.Payment.GroupId == groupId).Sum(t => (double?)t.Amount) ?? 0
                // We need to cast to (double?) because the result of the expression is NULL when there are no groups
                // and null is not assignable in the property of double
            };
        }
    }
}
