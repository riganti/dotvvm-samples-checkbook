using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CheckBook.DataAccess.Context;
using CheckBook.DataAccess.Data;
using CheckBook.DataAccess.Enums;
using CheckBook.DataAccess.Model;
using DotVVM.Framework.Controls;

namespace CheckBook.DataAccess.Services
{
    public static class PaymentService
    {
        /// <summary>
        /// Loads all payments in the specified group.
        /// </summary>
        public static void LoadPayments(int groupId, GridViewDataSet<PaymentData> dataSet)
        {
            using (var db = new AppDbContext())
            {
                var payments = db.Payments
                    .Where(p => p.GroupId == groupId && !p.IsDeleted)
                    .Select(ToPaymentData);

                // This handles sorting and paging for you. Just give the IQueryable<T> into the dataSet's LoadFromQueryable method
                dataSet.LoadFromQueryable(payments);
            }
        }

        public static void LoadMyTransactions(int userId, GridViewDataSet<MyTransactionData> dataSet)
        {
            using (var db = new AppDbContext())
            {
                var payments = db.Payments
                    .Where(p => !p.IsDeleted && p.Transactions.Any(t => t.UserId == userId))
                    .OrderByDescending(p => p.CreatedDate)
                    .Select(ToMyTransactionData(userId));

                // This handles sorting and paging for you. Just give the IQueryable<T> into the dataSet's LoadFromQueryable method
                dataSet.LoadFromQueryable(payments);
            }
        }

        public static void LoadMyPaymentLog(int userId, GridViewDataSet<PaymentLogData> dataSet, List<LogType> allowedLogTypes)
        {
            using (var db = new AppDbContext())
            {
                var payments = db.PaymentLogs
                    .Where(p => p.UserId == userId && allowedLogTypes.Contains(p.LogType))
                    .OrderByDescending(p => p.CreatedDate)
                    .Select(ToPaymentLogData());

                // This handles sorting and paging for you. Just give the IQueryable<T> into the dataSet's LoadFromQueryable method
                dataSet.LoadFromQueryable(payments);
            }
        }

        public static void LoadAllPaymentLogs(GridViewDataSet<PaymentLogData> dataSet, List<LogType> allowedLogTypes)
        {
            using (var db = new AppDbContext())
            {
                var payments = db.PaymentLogs
                    .Where(p => allowedLogTypes.Contains(p.LogType))
                    .OrderByDescending(p => p.CreatedDate)
                    .Select(ToPaymentLogData());

                // This handles sorting and paging for you. Just give the IQueryable<T> into the dataSet's LoadFromQueryable method
                dataSet.LoadFromQueryable(payments);
            }
        }

        /// <summary>
        /// Gets the payment by ID.
        /// </summary>
        public static PaymentData GetPayment(int paymentId)
        {
            using (var db = new AppDbContext())
            {
                return db.Payments
                    .Where(pg => pg.Id == paymentId)
                    .Select(ToPaymentData)
                    .Single();
            }
        }

        /// <summary>
        /// Gets a list of all users in the specified group with paid amounts from the specified payment.
        /// </summary>
        public static List<TransactionData> GetPayers(int groupId, int paymentId)
        {
            using (var db = new AppDbContext())
            {
                return db.Transactions
                    .Where(t => t.PaymentId == paymentId)
                    .Where(t => t.Type == TransactionType.Payment)
                    .Where(t => t.Amount >= 0)
                    .OrderBy(t => t.User.FirstName).ThenBy(t => t.User.LastName)
                    .Select(t => new TransactionData()
                    {
                        Amount = t.Amount,
                        UserId = t.UserId
                    })
                    .ToList();
            }
        }

        /// <summary>
        /// Gets a list of all users in the specified group with debt amounts from the specified payment.
        /// </summary>
        public static List<TransactionData> GetDebtors(int groupId, int paymentId)
        {
            using (var db = new AppDbContext())
            {
                return db.Transactions
                    .Where(t => t.PaymentId == paymentId)
                    .Where(t => t.Type == TransactionType.Payment)
                    .Where(t => t.Amount < 0)
                    .OrderBy(t => t.User.FirstName).ThenBy(t => t.User.LastName)
                    .Select(t => new TransactionData()
                    {
                        Amount = -t.Amount,
                        UserId = t.UserId
                    })
                    .ToList();
            }
        }

        /// <summary>
        /// Saves the payment with all details (who paid how much for who)
        /// </summary>
        public static void SavePayment(int userId, PaymentData data, List<TransactionData> payers, List<TransactionData> debtors)
        {
            using (var db = new AppDbContext())
            {
                // get or create the payment
                var payment = db.Payments.Find(data.Id);
                var transactions = new List<Transaction>();
                if (payment == null)
                {
                    payment = new Payment()
                    {
                        GroupId = data.GroupId
                    };
                    db.Payments.Add(payment);
                }
                else
                {
                    // check permissions
                    if (!IsPaymentEditable(userId, data.Id))
                    {
                        throw new UnauthorizedAccessException("You don't have permissions to modify the payment!");
                    }
                    transactions.AddRange(payment.Transactions);
                }

                payment.CreatedDate = data.CreatedDate;
                payment.Description = data.Description;

                // delete all current payments
                foreach (var transaction in payment.Transactions.ToList())
                {
                    db.Transactions.Remove(transaction);
                }

                // generate new payments
                var involvedUsers = new HashSet<int>();
                foreach (var payer in payers.Where(p => p.UserId != null && p.Amount != null && p.Amount != 0))
                {
                    payment.Transactions.Add(new Transaction()
                    {
                        Amount = payer.Amount.Value,
                        UserId = payer.UserId.Value,
                        Type = TransactionType.Payment
                    });
                    involvedUsers.Add(payer.UserId.Value);
                }

                foreach (var debtor in debtors.Where(p => p.UserId != null && p.Amount != null && p.Amount != 0))
                {
                    payment.Transactions.Add(new Transaction()
                    {
                        Amount = -debtor.Amount.Value,
                        UserId = debtor.UserId.Value,
                        Type = TransactionType.Payment
                    });
                    involvedUsers.Add(debtor.UserId.Value);
                }

                // calculate rounding
                var difference = (payers.Sum(p => p.Amount) ?? 0) - (debtors.Sum(p => p.Amount) ?? 0);
                if (difference != 0 && involvedUsers.Any())
                {
                    foreach (var user in involvedUsers)
                    {
                        payment.Transactions.Add(new Transaction()
                        {
                            Amount = -difference / involvedUsers.Count,
                            UserId = user,
                            Type = TransactionType.AutoGeneratedRounding
                        });
                    }
                }

                if (involvedUsers.Count < 2)
                {
                    // no data was entered
                    throw new Exception("You have to fill the amount for at least two users!");
                }

                //log changes
                db.PaymentLogs.AddRange(BuildPaymentLogs(transactions.Any() ? LogType.Edit : LogType.Create, userId,
                    transactions, payment.Transactions.ToList(), payment.Id));

                db.SaveChanges();
            }
        }

        private static ICollection<PaymentLog> BuildPaymentLogs(LogType logType, int editorId, ICollection<Transaction> fromTransactions,
            ICollection<Transaction> toTransactions, int paymentId)
        {
            var paymentLogs = new List<PaymentLog>();

            var mergedFromTransactions = fromTransactions.Concat(
                toTransactions.Select(t => new Transaction
                {
                    UserId = t.UserId,
                    Amount = 0
                })).GroupBy(t => t.UserId).ToList();
            var mergedToTransactions = toTransactions.GroupBy(t => t.UserId).ToList();

            foreach (var user in mergedFromTransactions)
            {
                paymentLogs.Add(new PaymentLog()
                {
                    EditorId = editorId,
                    UserId = user.Key,
                    AmountOriginal = CalculateAmount(user),
                    CreatedDate = DateTime.Now,
                    AmountNew = CalculateAmount(mergedToTransactions.FirstOrDefault(t => t.Key == user.Key)),
                    LogType = logType,
                    PaymentId = paymentId
                });
            }

            return paymentLogs.Where(t => decimal.Round(t.AmountOriginal, 1) != decimal.Round(t.AmountNew, 1)).ToList();
        }

        private static decimal CalculateAmount(IGrouping<int, Transaction> user)
        {
            return user?.Sum(transaction => transaction.Amount) ?? 0;
        }

        /// <summary>
        /// Deletes the payment.
        /// </summary>
        public static void DeletePayment(int userId, PaymentData data)
        {
            using (var db = new AppDbContext())
            {
                // check permissions
                if (!IsPaymentEditable(userId, data.Id))
                {
                    throw new UnauthorizedAccessException("You don't have permissions to delete the payment!");
                }

                // get the payment
                var payment = db.Payments.Find(data.Id);
                if (payment == null)
                {
                    throw new Exception("Could not find the payment");
                } 
                
                //log changes
                db.PaymentLogs.AddRange(BuildPaymentLogs(LogType.Delete, userId, payment.Transactions.ToList(),
                    new List<Transaction>(), payment.Id));

                //delete payment
                payment.IsDeleted = true;
                foreach (var transaction in payment.Transactions.ToList())
                {
                    db.Transactions.Remove(transaction);
                }

                db.SaveChanges();
            }
        }

        /// <summary>
        /// Determines whether the user can edit the specified payment.
        /// </summary>
        public static bool IsPaymentEditable(int userId, int paymentId)
        {
            using (var db = new AppDbContext())
            {
                var user = db.Users.Find(userId);
                return user?.UserRole == UserRole.Admin
                       || db.Payments.Any(pg => pg.Id == paymentId && pg.Group.UserGroups.Any(ug => ug.UserId == userId));
            }
        }

        private static Expression<Func<Payment, PaymentData>> ToPaymentData
        {
            get
            {
                return pg => new PaymentData()
                {
                    Id = pg.Id,
                    Description = pg.Description,
                    TotalAmount = pg.Transactions.Where(p => p.Amount > 0).Sum(p => (decimal?)p.Amount) ?? 0,
                    CreatedDate = pg.CreatedDate,
                    Currency = pg.Group.Currency,
                    GroupId = pg.GroupId
                };
            }
        }

        private static Expression<Func<Payment, MyTransactionData>> ToMyTransactionData(int userId)
        {
            return p => new MyTransactionData()
            {
                PaymentId = p.Id,
                Description = p.Description,
                TotalAmount = p.Transactions.Where(t => t.Amount > 0).Sum(t => (decimal?)t.Amount) ?? 0,
                CreatedDate = p.CreatedDate,
                Currency = p.Group.Currency,
                GroupId = p.GroupId,
                GroupName = p.Group.Name,
                MyBalance = p.Transactions.Where(t => t.UserId == userId).Sum(t => (decimal?)t.Amount) ?? 0,
                MySpending = p.Transactions.Where(t => t.UserId == userId && t.Amount < 0).Sum(t => (decimal?)t.Amount) ?? 0
            };
        }

        private static Expression<Func<PaymentLog, PaymentLogData>> ToPaymentLogData()
        {
            return p => new PaymentLogData
            {
                EditDate = p.CreatedDate,
                PaymentCreatedDate = p.Payment.CreatedDate,
                PaymentDescription = p.Payment.Description,
                EditorName = p.Editor.FirstName + " " + p.Editor.LastName,
                UserName = p.User.FirstName + " " + p.User.LastName,
                LogType = p.LogType,
                Currency = p.Payment.Group.Currency,
                AmountOriginal = p.AmountOriginal,
                AmountNew = p.AmountNew
            };
        }
    }
}
