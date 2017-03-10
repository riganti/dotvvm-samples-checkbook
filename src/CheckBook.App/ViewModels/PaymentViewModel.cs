using System;
using DotVVM.Framework.Controls;
using DotVVM.Framework.Runtime.Filters;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.UI;
using CheckBook.DataAccess.Data;
using CheckBook.DataAccess.Services;
using DotVVM.Framework.ViewModel;

namespace CheckBook.App.ViewModels
{
    [Authorize]
    public class PaymentViewModel : AppViewModelBase
    {
        public override string ActivePage => "home";

        public PaymentData Data { get; set; }

        public List<TransactionData> AllPayers { get; set; }

        public List<TransactionData> AllDebtors { get; set; }

        public decimal AmountDifference { get; set; }
        
        public string ErrorMessage { get; set; }
        
        [Protect(ProtectMode.SignData)]
        public bool IsEditable { get; set; }

        public bool IsDeletable { get; set; }

        [Bind(Direction.ClientToServerNotInPostbackPath)]
        public string AutoComplete { get; set; }

        public List<TransactionRowData> Payers { get; set; } = new List<TransactionRowData>();

        public List<TransactionRowData> Debtors { get; set; } = new List<TransactionRowData>();

        public List<string> Names { get; set; } = new List<string>();

        public string GroupName { get; set; }

        public override Task Load()
        {
            if (!Context.IsPostBack)
            {
                CreateOrLoadData();
            }
            return base.Load();
        }

        /// <summary>
        /// Loads the data in the form
        /// </summary>
        private void CreateOrLoadData()
        {
            // get group
            var userId = GetUserId();
            var groupId = Convert.ToInt32(Context.Parameters["GroupId"]);
            var group = GroupService.GetGroup(groupId, userId);

            // get or create the payment
            var paymentId = Context.Parameters["Id"];
            if (paymentId != null)
            {
                // load
                Data = PaymentService.GetPayment(Convert.ToInt32(paymentId));
                IsEditable = IsDeletable = PaymentService.IsPaymentEditable(userId, Convert.ToInt32(paymentId));
            }
            else
            {
                // create new
                Data = new PaymentData()
                {
                    GroupId = groupId,
                    CreatedDate = DateTime.Today,
                    Currency = group.Currency
                };
                IsEditable = true;
                IsDeletable = false;
            }

            GroupName = group.Name;

            // load payers and debtors
            AllPayers = PaymentService.GetPayers(groupId, Convert.ToInt32(paymentId));
            AllDebtors = PaymentService.GetDebtors(groupId, Convert.ToInt32(paymentId));
            Recalculate();

            // add first rows to form
            AddRow(Payers);
            AddRow(Debtors);
        }

        /// <summary>
        /// Recalculates the remaining amount.
        /// </summary>
        public void Recalculate()
        {
            AmountDifference = (Payers.Where(p => p.Amount != null).Sum(p => p.Amount) ?? 0) - (Debtors.Where(p => p.Amount != null).Sum(p => p.Amount) ?? 0);
        }

        /// <summary>
        /// Saves the payment.
        /// </summary>
        public void Save()
        {
            try
            {
                var userId = GetUserId();
                PaymentService.SavePayment(userId, Data, Payers, Debtors);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return;
            }

            GoBack();
        }

        /// <summary>
        /// Deletes the current payment.
        /// </summary>
        public void Delete()
        {
            try
            {
                var userId = GetUserId();
                PaymentService.DeletePayment(userId, Data, AllPayers, AllDebtors);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return;
            }

            GoBack();
        }

        /// <summary>
        /// Redirects back to the group page.
        /// </summary>
        public void GoBack()
        {
            Context.RedirectToRoute("group", new { Id = Data.GroupId });
        }


        //[Bind(Direction.ServerToClient)]
        public void FilterNames(string s, TransactionRowData row)
        {
            Names = AllPayers.Where(n => n.Name.ToLower().Contains(s.ToLower()))
                .Select(n => n.Name).ToList();
        }

        public void AddSelectedUser(string name, TransactionRowData row, string context)
        {
            if(context == "payers")
            {
                var user = AllPayers.Where(n => n.Name == name).First();
                var item = Payers.Where(n => n.RowId == row.RowId).First();
                item.Name = user.Name;
                item.UserId = user.UserId;
                item.ImageUrl = user.ImageUrl;
                item.Id = user.Id;
                item.IsUserboxVisible = true;
            }
            else if (context == "debtors")
            {
                var user = AllDebtors.Where(n => n.Name == name).First();
                var item = Debtors.Where(n => n.RowId == row.RowId).First();
                item.Name = user.Name;
                item.UserId = user.UserId;
                item.ImageUrl = user.ImageUrl;
                item.Id = user.Id;
                item.IsUserboxVisible = true;
            }

        }

        public void AddRow(List<TransactionRowData> list)
        {
            list.Add(new TransactionRowData() { RowId = list.Count });
        }
    }
}

