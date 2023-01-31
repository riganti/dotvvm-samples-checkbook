using System.Collections.Generic;
using DotVVM.Framework.Runtime.Filters;
using DotVVM.Framework.Controls;
using System.Threading.Tasks;
using CheckBook.DataAccess.Data;
using CheckBook.DataAccess.Services;
using System.Linq;
using DotVVM.Framework.Hosting;

namespace CheckBook.App.ViewModels
{
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

        public override async Task Init()
        {
            await Context.Authorize();

            await base.Init();
        }

        public override Task PreRender()
        {
            var userId = GetUserId();
            Groups = groupService.GetGroupsByUser(userId);

            return base.PreRender();
        }
    }
}
