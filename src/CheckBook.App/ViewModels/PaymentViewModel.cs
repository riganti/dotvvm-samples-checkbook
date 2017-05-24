using System;
using System.Collections;
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
        
        public List<string> FilteredNames { get; set; }

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


            foreach (var row in AllPayers.Where(n => n.Amount > 0))
            {
                AddLoadedUser(row, Payers);
            }
            foreach (var row in AllDebtors.Where(n => n.Amount > 0))
            {
                AddLoadedUser(row, Debtors);
            }


            if (AllPayers.Count > Payers.Count)
                AddRow(Payers);

            if (AllDebtors.Count > Debtors.Count)
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
        

        public void FilterPayersNames(string filter)
        {
            IEnumerable<TransactionData> payers = AllPayers;

            if (!string.IsNullOrWhiteSpace(filter))
                payers = payers.Where(t => t.Name.Contains(filter));
            
            FilteredNames = payers.Select(t=>t.Name) //select only names
                .Concat(Payers.Select(t=>t.Name))   //merge Names of allready added payers with names of all payers
                .GroupBy(t=>t).Where(g => g.Count() == 1).Select(g => g.Key).ToList(); //select only unique entries

            MoveCurrentUserToTop(FilteredNames);

        }

        public void FilterDebtorsNames(string filter)
        {
            IEnumerable<TransactionData> debtorsDatas = AllDebtors;

            if (!string.IsNullOrWhiteSpace(filter))
                debtorsDatas = debtorsDatas.Where(t => t.Name.Contains(filter));

            FilteredNames = debtorsDatas.Select(t => t.Name) //select only names
                .Concat(Debtors.Select(t => t.Name))   //merge Names of allready added debtors with names of all debtors
                .GroupBy(t => t).Where(g => g.Count() == 1).Select(g => g.Key).ToList(); //select only unique entries

            MoveCurrentUserToTop(FilteredNames);
        }

        protected void MoveCurrentUserToTop(List<string> collection)
        {
            var currentUserName = GetUserName();

            var index = collection.IndexOf(currentUserName);

            if (index == -1) return; //not found

            collection.RemoveAt(index);
            collection.Insert(0, currentUserName);
        }

        public void AddSelectedUser(string name, TransactionRowData row, string context)
        {
            
            if(context == "payers")
            {
                var user = AllPayers.FirstOrDefault(n => n.Name == name);
                if(user != null)
                {
                    var item = Payers.First(n => n.RowId == row.RowId);
                    item.Name = user.Name;
                    item.UserId = user.UserId;
                    item.ImageUrl = user.ImageUrl;
                    item.Id = user.Id;
                    item.IsUserboxVisible = true;
                }

                if (AllPayers.Count > Payers.Count)
                    AddRow(Payers);
            }
            else if (context == "debtors")
            {
                var user = AllDebtors.FirstOrDefault(n => n.Name == name);
                if (user != null)
                {
                    var item = Debtors.First(n => n.RowId == row.RowId);
                    item.Name = user.Name;
                    item.UserId = user.UserId;
                    item.ImageUrl = user.ImageUrl;
                    item.Id = user.Id;
                    item.IsUserboxVisible = true;
                }


                if (AllDebtors.Count > Debtors.Count)
                    AddRow(Debtors);
            }

        }
        public void AddLoadedUser(TransactionData user, List<TransactionRowData> list)
        {
            var item = new TransactionRowData
            {
                Name = user.Name,
                UserId = user.UserId,
                ImageUrl = user.ImageUrl,
                Id = user.Id,
                Amount = user.Amount,
                IsUserboxVisible = true
            };

            list.Add(item);
        }

        public void AddRow(List<TransactionRowData> list)
        {
            list.Add(new TransactionRowData() { RowId = list.Count });
        }

        public void EditRow(TransactionRowData row, string context)
        {
            if (context == "payers")
            {
                var item = Payers.Find(n => n == row);
                item.Name = "";
                item.IsUserboxVisible = false;

            }
            else if (context == "debtors")
            {
                var item = Debtors.Find(n => n == row);
                item.Name = "";
                item.IsUserboxVisible = false;

            }
        }
    }
}

