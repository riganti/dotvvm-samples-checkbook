using System;
using System.Collections.Generic;
using System.Linq;
using CheckBook.DataAccess.Data;

namespace CheckBook.DataAccess.Services
{
    public class SettlementService
    {

        /// <summary>
        /// Calculates the transactions to settle the members.
        /// </summary>
        public IEnumerable<SettlementData> CalculateSettlement(List<GroupMemberData> members)
        {
            // sort members by amounts and exclude 
            var sortedMembers = members
                .Where(m => Math.Abs(m.Amount) >= 1)
                .OrderBy(m => m.Amount)
                .Select(m => new TempMemberData()
                {
                    Name = m.Name,
                    Amount = m.Amount
                })
                .ToList();

            // generate transactions until the list is empty
            while (sortedMembers.Count > 1)
            {
                // get the first member (he owes the most) and send his money to the last one (he paid most)
                var first = sortedMembers[0];
                var last = sortedMembers[sortedMembers.Count - 1];
                var amountToSend = Math.Min(-first.Amount, last.Amount);

                // generate the transaction
                yield return new SettlementData()
                {
                    Name1 = first.Name,
                    Name2 = last.Name,
                    Amount = amountToSend
                };

                // update the members and sort again
                first.Amount += amountToSend;
                if (Math.Abs(first.Amount) < 1)
                {
                    sortedMembers.RemoveAt(0);
                }
                last.Amount -= amountToSend;
                if (Math.Abs(last.Amount) < 1)
                {
                    sortedMembers.RemoveAt(sortedMembers.Count - 1);
                }
                sortedMembers = sortedMembers
                    .OrderBy(m => m.Amount)
                    .ToList();
            }
        }

        private class TempMemberData
        {
            public string Name { get; set; }
            public double Amount { get; set; }
        }
    }
}