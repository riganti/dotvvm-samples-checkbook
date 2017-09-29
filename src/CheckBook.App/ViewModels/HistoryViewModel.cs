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

        public override Task PreRender()
	    {
            var userId = GetUserId();
            PaymentService.LoadMyTransactions(userId, MyTransactions);

            return base.PreRender();
	    }
	}
}

