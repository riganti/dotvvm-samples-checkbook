using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckBook.DataAccess.Data;
using CheckBook.DataAccess.Services;
using DotVVM.Framework.Controls;
using DotVVM.Framework.Hosting;
using DotVVM.Framework.Runtime.Filters;
using DotVVM.Framework.ViewModel;

namespace CheckBook.App.ViewModels
{
    public class GroupViewModel : AppViewModelBase
    {

        public override async Task Init()
        {
            await Context.Authorize();

            await base.Init();
        }

        private readonly GroupService groupService;
        private readonly PaymentService paymentService;
        private readonly SettlementService settlementService;

        public override string ActivePage => "home";

        [FromRoute("id")]
        public int GroupId { get; private set; }

        public string GroupName { get; private set; }

        public string Currency { get; private set; }

        public List<GroupMemberData> Members { get; private set; }

        public List<SettlementData> Settlement { get; private set; }

        public GridViewDataSet<PaymentData> Payments { get; set; } = new GridViewDataSet<PaymentData>()
        {
            PagingOptions =
            {
                PageSize = 40
            },
            SortingOptions =
            {
                SortDescending = true,
                SortExpression = nameof(PaymentData.CreatedDate)
            }
        };

        public GroupViewModel(GroupService groupService, PaymentService paymentService, SettlementService settlementService)
        {
            this.groupService = groupService;
            this.paymentService = paymentService;
            this.settlementService = settlementService;
        }

        public override Task PreRender()
        {
            // load group name
            var userId = GetUserId();
            var group = groupService.GetGroup(GroupId, userId);
            GroupName = group.Name;
            Currency = group.Currency;

            // load payments in current group
            paymentService.LoadPayments(GroupId, Payments);

            // load members
            Members = groupService.GetGroupMembers(GroupId);

            // generate settlements
            Settlement = settlementService.CalculateSettlement(Members).ToList();

            return base.PreRender();
        }
    }
}

