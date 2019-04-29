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
    public class HistoryViewModel : AppViewModelBase
	{
        private readonly PaymentService paymentService;

        public GridViewDataSet<MyTransactionData> MyTransactions { get; set; } = new GridViewDataSet<MyTransactionData>()
        {
            PagingOptions =
            {
                PageSize = 20
            },
            SortingOptions =
            {
                SortExpression = nameof(MyTransactionData.CreatedDate),
                SortDescending = true
            }
        };

        public HistoryViewModel(PaymentService paymentService)
        {
            this.paymentService = paymentService;
        }

        public override Task PreRender()
	    {
            var userId = GetUserId();
            paymentService.LoadMyTransactions(userId, MyTransactions);

            return base.PreRender();
	    }
	}
}

