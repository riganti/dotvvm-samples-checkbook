using System;
using System.Collections.Generic;
using System.Linq;
using DotVVM.Framework.Controls;
using System.Threading.Tasks;
using CheckBook.DataAccess.Data;
using CheckBook.DataAccess.Enums;
using CheckBook.DataAccess.Services;
using DotVVM.Framework.Runtime.Filters;
using DotVVM.Framework.ViewModel;

namespace CheckBook.App.ViewModels
{
    [Authorize(Roles = nameof(UserRole.Admin))]
    public class ManagerViewModel : AppViewModelBase
    {
        private readonly GroupService groupService;
        private readonly UserService userService;

        public GridViewDataSet<UserInfoData> Users { get; set; } = new GridViewDataSet<UserInfoData>()
        {
            PagingOptions =
            {
                PageSize = 20
            },
            SortingOptions =
            {
                SortExpression = nameof(UserInfoData.LastName),
                SortDescending = false
            }
        };

        public UserInfoData EditedUser { get; set; } = new UserInfoData();

        public string UserAlertText { get; set; }


        public GridViewDataSet<GroupData> Groups { get; set; } = new GridViewDataSet<GroupData>()
        {
            PagingOptions =
            {
                PageSize = 20
            },
            SortingOptions =
            {
                SortExpression = nameof(GroupData.Name),
                SortDescending = false
            }
        };

        public GroupData EditedGroup { get; set; } = new GroupData();

        [Bind(Direction.ServerToClient)]
        public string GroupAlertText { get; private set; }

        public string GroupSearchText { get; set; }

        public List<UserInfoData> GroupSearchResults { get; set; }

        public List<UserInfoData> GroupUsers { get; set; }

        public ManagerViewModel(GroupService groupService, UserService userService)
        {
            this.groupService = groupService;
            this.userService = userService;
        }

        public override Task PreRender()
        {
            userService.LoadUserInfos(Users);
            groupService.LoadGroups(Groups);

            return base.PreRender();
        }

        /// <summary>
        /// Shows the user detail popup.
        /// </summary>
        public void ShowUserPopup(int? userId)
        {
            if (userId == null)
            {
                EditedUser = new UserInfoData() { UserRole = UserRole.User };
            }
            else
            {
                EditedUser = userService.GetUserInfo(userId.Value);
            }

            Context.ResourceManager.AddStartupScript("$('div[data-id=user-detail]').modal('show');");
        }

        /// <summary>
        /// Saves the changes in the user popup.
        /// </summary>
        public void SaveUser()
        {
            try
            {
                userService.CreateOrUpdateUserInfo(EditedUser);
                Context.ResourceManager.AddStartupScript("$('div[data-id=user-detail]').modal('hide');");
            }
            catch (Exception ex)
            {
                UserAlertText = ex.Message;
            }
        }

        /// <summary>
        /// Deletes the user.
        /// </summary>
        public void DeleteUser()
        {
            try
            {
                userService.DeleteUser(EditedUser.Id);
                Context.ResourceManager.AddStartupScript("$('div[data-id=user-detail]').modal('hide');");
            }
            catch (Exception ex)
            {
                UserAlertText = ex.Message;
            }
        }

        /// <summary>
        /// Shows the group detail popup.
        /// </summary>
        public void ShowGroupPopup(int? groupId)
        {
            if (groupId == null)
            {
                EditedGroup = new GroupData();
                GroupUsers = new List<UserInfoData>();
            }
            else
            {
                EditedGroup = groupService.GetGroup(groupId.Value);
                GroupUsers = userService.GetGroupUsers(groupId.Value);
            }

            // load users
            GroupSearchText = "";
            GroupSearch();

            Context.ResourceManager.AddStartupScript("$('div[data-id=group-detail]').modal('show');");
        }

        /// <summary>
        /// Searches for the user from the group detail search box.
        /// </summary>
        public void GroupSearch()
        {
            var currentGroupUsers = new HashSet<int>(GroupUsers.Select(u => u.UserId ?? 0));

            GroupSearchResults = userService.SearchUsers(GroupSearchText)
                .Where(u => !currentGroupUsers.Contains(u.UserId ?? 0))
                .ToList();
        }

        /// <summary>
        /// Adds the user to currently edited group.
        /// </summary>
        public void GroupAddUser(UserInfoData user)
        {
            if (!GroupUsers.Any(u => u.UserId == user.Id))
            {
                GroupUsers.Add(user);
            }
            GroupSearch();
        }

        /// <summary>
        /// Removes the user from currently edited group.
        /// </summary>
        public void GroupRemoveUser(UserInfoData user)
        {
            GroupUsers.Remove(user);
            GroupSearch();
        }

        /// <summary>
        /// Saves the changes made to the currently edited group.
        /// </summary>
        public void SaveGroup()
        {
            try
            {
                groupService.CreateOrUpdateGroup(EditedGroup, GroupUsers);
                Context.ResourceManager.AddStartupScript("$('div[data-id=group-detail]').modal('hide');");
            }
            catch (Exception ex)
            {
                GroupAlertText = ex.Message;
            }
        }

        /// <summary>
        /// Deletes the currently edited group.
        /// </summary>
        public void DeleteGroup()
        {
            try
            {
                groupService.DeleteGroup(EditedGroup.Id);
                Context.ResourceManager.AddStartupScript("$('div[data-id=group-detail]').modal('hide');");
            }
            catch (Exception ex)
            {
                GroupAlertText = ex.Message;
            }
        }
    }
}
