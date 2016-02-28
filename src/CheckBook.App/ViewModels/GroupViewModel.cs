using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckBook.DataAccess.Data;
using CheckBook.DataAccess.Services;
using DotVVM.Framework.Controls;
using DotVVM.Framework.Runtime.Filters;

namespace CheckBook.App.ViewModels
{
    [Authorize]
    public class GroupViewModel : AppViewModelBase
	{
	    public override string ActivePage => "home";

        public int GroupId => Convert.ToInt32(Context.Parameters["Id"]);

        public string GroupName { get; private set; }

        public string Currency { get; private set; }

        public List<GroupMemberData> Members { get; private set; }

        public List<SettlementData> Settlement { get; private set; }

        public GridViewDataSet<PaymentData> Payments { get; set; } = new GridViewDataSet<PaymentData>()
        {
            PageSize = 40,
            SortDescending = true,
            SortExpression = nameof(PaymentData.CreatedDate)
        };


        public override Task PreRender()
	    {
            // load group name
	        var userId = GetUserId();
            var group = GroupService.GetGroup(GroupId, userId);
            GroupName = group.Name;
            Currency = group.Currency;

            // load payments in current group
            PaymentService.LoadPayments(GroupId, Payments);

            // load members
            Members = GroupService.GetGroupMembers(GroupId);

            // generate settlements
            Settlement = SettlementService.CalculateSettlement(Members).ToList();

	        return base.PreRender();
	    }
	}
}

