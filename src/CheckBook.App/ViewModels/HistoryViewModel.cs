using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckBook.DataAccess.Data;
using CheckBook.DataAccess.Services;
using DotVVM.Framework.Controls;

namespace CheckBook.App.ViewModels
{
	public class HistoryViewModel : AppViewModelBase
	{

        public GridViewDataSet<MyTransactionData> MyTransactions { get; set; } = new GridViewDataSet<MyTransactionData>()
        {
            SortExpression = nameof(MyTransactionData.CreatedDate),
            SortDescending = true,
            PageSize = 20
        };

        public override Task PreRender()
	    {
            var userId = GetUserId();
            PaymentService.LoadMyTransactions(userId, MyTransactions);

            return base.PreRender();
	    }
	}
}

