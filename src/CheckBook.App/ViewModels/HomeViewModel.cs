using System.Collections.Generic;
using DotVVM.Framework.Runtime.Filters;
using DotVVM.Framework.Controls;
using System.Threading.Tasks;
using CheckBook.DataAccess.Data;
using CheckBook.DataAccess.Services;
using System.Linq;

namespace CheckBook.App.ViewModels
{
    [Authorize]
	public class HomeViewModel : AppViewModelBase
    {
        private readonly GroupService groupService;

        /// <summary>
        /// Gets the list of groups the current user is assigned in.
        /// </summary>
        public List<GroupData> Groups { get; private set; }

        public HomeViewModel(GroupService groupService)
        {
            this.groupService = groupService;
        }

        public override Task PreRender()
        {
            var userId = GetUserId();
            Groups = groupService.GetGroupsByUser(userId);

            return base.PreRender();
        }
    }
}
