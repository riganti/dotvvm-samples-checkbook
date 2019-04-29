using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckBook.DataAccess.Data;
using CheckBook.DataAccess.Enums;
using CheckBook.DataAccess.Model;
using CheckBook.DataAccess.Services;
using DotVVM.Framework.Controls;
using DotVVM.Framework.Runtime.Filters;
using DotVVM.Framework.ViewModel;

namespace CheckBook.App.ViewModels
{
    [Authorize]
    public class ChangelogViewModel : AppViewModelBase
    {
        private readonly PaymentService paymentService;

        public GridViewDataSet<PaymentLogData> MyPaymentLog { get; set; } = new GridViewDataSet<PaymentLogData>()
        {

            PagingOptions =
            {
                PageSize = 20
            },
            SortingOptions =
            {
                SortExpression = nameof(PaymentLogData.EditDate),
                SortDescending = true
            }
        };

        public bool ShowAllUsers { get; set; }
        public bool ShowCreations { get; set; }
        public bool ShowDeletions { get; set; }
        public bool ShowEdits { get; set; }


        public ChangelogViewModel(PaymentService paymentService)
        {
            this.paymentService = paymentService;
        }

        public override Task Init()
        {
            if (!Context.IsPostBack)
            {
                ShowCreations = false;
                ShowDeletions = true;
                ShowEdits = true;
            }
            return base.Init();
        }

        public override Task PreRender()
        {
            if (ShowAllUsers)
            {
                paymentService.LoadAllPaymentLogs(MyPaymentLog, GetAllowedLogTypes());
            }
            else
            {
                var userId = GetUserId();
                paymentService.LoadMyPaymentLog(userId, MyPaymentLog, GetAllowedLogTypes());
            }
            return base.PreRender();
        }

        public void ShowAllUsersChanged() {}

        private List<LogType> GetAllowedLogTypes()
        {
            var result = new List<LogType>();

            if (ShowCreations)
            {
                result.Add(LogType.Create);
            }
            if (ShowDeletions)
            {
                result.Add(LogType.Delete);
            }
            if(ShowEdits)
            {
               result.Add(LogType.Edit);
            }

            return result;
        }
    }
}

