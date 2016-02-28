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
            using (var db = new AppContext())
            {
                var payments = db.Payments
                    .Where(pg => pg.GroupId == groupId)
                    .Select(ToPaymentData);

                // This handles sorting and paging for you. Just give the IQueryable<T> into the dataSet's LoadFromQueryable method
                dataSet.LoadFromQueryable(payments);
            }
        }

        /// <summary>
        /// Gets the payment by ID.
        /// </summary>
        public static PaymentData GetPayment(int paymentId)
        {
            using (var db = new AppContext())
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
            using (var db = new AppContext())
            {
                return db.Users
                    .Where(u => u.UserGroups.Any(ug => ug.GroupId == groupId))
                    .Select(u => new
                    {
                        User = u,
                        Payment = u.Transactions.FirstOrDefault(p => p.PaymentId == paymentId && p.Type == TransactionType.Payment && p.Amount >= 0)
                    })
                    .Select(u => new TransactionData()
                    {
                        Id = u.Payment.Id,
                        Name = u.User.FirstName + " " + u.User.LastName,
                        Amount = u.Payment.Amount,
                        UserId = u.User.Id,
                        ImageUrl = u.User.ImageUrl
                    })
                    .OrderBy(u => u.Name)
                    .ToList();
            }
        }

        /// <summary>
        /// Gets a list of all users in the specified group with debt amounts from the specified payment.
        /// </summary>
        public static List<TransactionData> GetDebtors(int groupId, int paymentId)
        {
            using (var db = new AppContext())
            {
                return db.Users
                    .Where(u => u.UserGroups.Any(ug => ug.GroupId == groupId))
                    .Select(u => new
                    {
                        User = u,
                        Payment = u.Transactions.FirstOrDefault(p => p.PaymentId == paymentId && p.Type == TransactionType.Payment && p.Amount < 0)
                    })
                    .Select(u => new TransactionData()
                    {
                        Id = u.Payment.Id,
                        Name = u.User.FirstName + " " + u.User.LastName,
                        Amount = -u.Payment.Amount,
                        UserId = u.User.Id,
                        ImageUrl = u.User.ImageUrl
                    })
                    .OrderBy(u => u.Name)
                    .ToList();
            }
        }

        /// <summary>
        /// Saves the payment with all details (who paid how much for who)
        /// </summary>
        public static void SavePayment(int userId, PaymentData data, List<TransactionData> payers, List<TransactionData> debtors)
        {
            using (var db = new AppContext())
            {
                // get or create the payment
                var payment = db.Payments.Find(data.Id);
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
                foreach (var payer in payers.Where(p => p.Amount != null && p.Amount != 0))
                {
                    payment.Transactions.Add(new Transaction()
                    {
                        Amount = payer.Amount.Value,
                        UserId = payer.UserId,
                        Type = TransactionType.Payment
                    });
                    involvedUsers.Add(payer.UserId);
                }
                foreach (var debtor in debtors.Where(p => p.Amount != null && p.Amount != 0))
                {
                    payment.Transactions.Add(new Transaction()
                    {
                        Amount = -debtor.Amount.Value,
                        UserId = debtor.UserId,
                        Type = TransactionType.Payment
                    });
                    involvedUsers.Add(debtor.UserId);
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
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Deletes the payment.
        /// </summary>
        public static void DeletePayment(int userId, PaymentData data, List<TransactionData> payers, List<TransactionData> debtors)
        {
            using (var db = new AppContext())
            {
                // check permissions
                if (!IsPaymentEditable(userId, data.Id))
                {
                    throw new UnauthorizedAccessException("You don't have permissions to delete the payment!");
                }

                // get the payment
                var payment = db.Payments.Find(data.Id);
                db.Payments.Remove(payment);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Determines whether the user can edit the specified payment.
        /// </summary>
        public static bool IsPaymentEditable(int userId, int paymentId)
        {
            using (var db = new AppContext())
            {
                var user = db.Users.Find(userId);
                return user.UserRole == UserRole.Admin 
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
    }
}
