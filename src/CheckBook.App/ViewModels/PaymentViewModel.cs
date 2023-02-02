using System;
using System.Collections;
using DotVVM.Framework.Controls;
using DotVVM.Framework.Runtime.Filters;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CheckBook.DataAccess.Data;
using CheckBook.DataAccess.Services;
using DotVVM.Framework.ViewModel;
using DotVVM.Framework.Hosting;

namespace CheckBook.App.ViewModels
{
    public class PaymentViewModel : AppViewModelBase
    {
        private readonly GroupService groupService;
        private readonly UserService userService;
        private readonly PaymentService paymentService;
        public override string ActivePage => "home";

        [FromRoute("GroupId")]
        public int GroupId { get; set; }

        [FromRoute("Id")]
        public int PaymentId { get; set; }

        public PaymentData Data { get; set; }

        public List<TransactionData> Payers { get; set; } = new List<TransactionData>();

        public List<TransactionData> Debtors { get; set; } = new List<TransactionData>();


        [Bind(Direction.ServerToClient)]
        public double AmountDifference { get; set; }


        [Bind(Direction.ServerToClient)]
        public string ErrorMessage { get; set; }

        [Protect(ProtectMode.SignData)]
        public bool IsEditable { get; set; }

        [Protect(ProtectMode.SignData)]
        public bool IsDeletable { get; set; }


        [Bind(Direction.ServerToClientFirstRequest)]
        public string GroupName { get; set; }

        [Bind(Direction.ServerToClientFirstRequest)]
        public List<UserBasicInfoData> AllUsers { get; set; }


        public PaymentViewModel(GroupService groupService, UserService userService, PaymentService paymentService)
        {
            this.groupService = groupService;
            this.userService = userService;
            this.paymentService = paymentService;
        }

        public override async Task Init()
        {
            await Context.Authorize();

            await base.Init();
        }
        public override Task Load()
        {
            // load all users
            AllUsers = userService.GetUserBasicInfoList(GroupId);

            // load data
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
            var group = groupService.GetGroup(GroupId, userId);

            // get or create the payment
            if (PaymentId > 0)
            {
                // load
                Data = paymentService.GetPayment(PaymentId);
                IsEditable = IsDeletable = paymentService.IsPaymentEditable(userId, PaymentId);
            }
            else
            {
                // create new
                Data = new PaymentData()
                {
                    GroupId = GroupId,
                    CreatedDate = DateTime.Today,
                    Currency = group.Currency
                };
                IsEditable = true;
                IsDeletable = false;
            }

            GroupName = group.Name;

            // load payers and debtors
            Payers = paymentService.GetPayers(GroupId, PaymentId);
            EnsureInsertRowPresent(Payers);
            UpdateNameAndImageUrl(Payers);

            Debtors = paymentService.GetDebtors(GroupId, PaymentId);
            EnsureInsertRowPresent(Debtors);
            UpdateNameAndImageUrl(Debtors);

            Recalculate();
        }

        private void EnsureInsertRowPresent(List<TransactionData> list)
        {
            var emptyRows = list.Where(r => r.UserId == null && r.Amount == null).ToList();

            if (emptyRows.Count == 0)
            {
                list.Add(new TransactionData());
            }
            else if (emptyRows.Count > 1)
            {
                foreach (var emptyRow in emptyRows.Skip(1))
                {
                    list.Remove(emptyRow);
                }
            }
            else if (list.Last() != emptyRows[0])
            {
                list.Remove(emptyRows[0]);
                list.Add(emptyRows[0]);
            }
        }

        private void UpdateNameAndImageUrl(List<TransactionData> list)
        {
            foreach (var row in list)
            {
                var user = AllUsers.FirstOrDefault(u => u.Id == row.UserId);
                row.Name = user?.Name;
                row.ImageUrl = user?.ImageUrl;
            }
        }

        public void PayersChanged()
        {
            UpdateNameAndImageUrl(Payers);
            EnsureInsertRowPresent(Payers);
            Recalculate();
        }

        public void DeletePayer(TransactionData payer)
        {
            Payers.Remove(payer);
            EnsureInsertRowPresent(Payers);
            Recalculate();
        }

        public void DebtorsChanged()
        {
            UpdateNameAndImageUrl(Debtors);
            EnsureInsertRowPresent(Debtors);
            Recalculate();
        }

        public void DeleteDebtor(TransactionData debtor)
        {
            Debtors.Remove(debtor);
            EnsureInsertRowPresent(Debtors);
            Recalculate();
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
                paymentService.SavePayment(userId, Data, Payers, Debtors);
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
                paymentService.DeletePayment(userId, Data);
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

        public void InvolveEveryone()
        {
            Recalculate();
            var alreadyPresentDebtors = Debtors.Where(t => t.UserId != null).Select(t => t.UserId).ToArray();
            var missingDebtors = AllUsers.Where(t => alreadyPresentDebtors.All(d => d != t.Id)).ToArray();
            var missingTransactions = missingDebtors.Select(t => new TransactionData() { UserId = t.Id, Name = t.Name, Amount = AmountDifference / missingDebtors.Count() });
            Debtors.AddRange(missingTransactions);

            DebtorsChanged();
        }
    }
}

