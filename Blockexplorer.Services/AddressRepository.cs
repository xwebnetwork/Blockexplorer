using Blockexplorer.Core.Repositories;
using System;
using Blockexplorer.Core.Domain;
using System.Threading.Tasks;
using Blockexplorer.Entities;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Blockexplorer.Services
{
    public class AddressRepository : IAddressRepository
    {
        public async Task<Address> GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;

            id = id.Trim();
            if (id.Length != 34 && !id.Equals("OP_RETURN", StringComparison.OrdinalIgnoreCase))
                return null;

            using (var db = new ObsidianChainContext())
            {
                var addressEntity = await db.AddressEntities.FindAsync(id);
                if (addressEntity == null)
                    return null;

                List<Transaction> transactions = new List<Transaction>();
                string[] txids = addressEntity.TxIdBlob.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
                foreach (var txid in txids)
                {
                    var transaction = new Transaction
                    {
                        TransactionId = txid
                    };
                    transactions.Add(transaction);

                }

                var address = new Address
                {
                    Id = id,
                    Balance = addressEntity.Balance,
                    LastModifiedBlockHeight = addressEntity.LastModifiedBlockHeight,
                    TxIds = txids,
                    TotalTransactions = txids.Length,
                    Transactions = null,
                    ColoredAddress = null
                };
                return address;
            }
        }

        public async Task<List<Address>> GetTopList()
        {
            using (var db = new ObsidianChainContext())
            {
                List<Address> addresses = await db.AddressEntities.AsNoTracking().OrderByDescending(adr => adr.Balance).Take(100)
                    .Select(adr => new { Id = adr.Id, Balance = adr.Balance })
                    .Select(res => new Address() { Id = res.Id, Balance = res.Balance })
                    .ToListAsync();
                return addresses;
            }
        }

        public async Task<Tuple<int, DateTime>> GetBestAddressIndexBlockHeight()
        {
            using (var db = new ObsidianChainContext())
            {
                var stats = await db.StatEntities.FindAsync("1");
                return new Tuple<int, DateTime>(stats.BestAdrIndexHeight, stats.ModifiedDate);
            }
        }

        public async Task Save(Address entity)
        {

        }

    }
}
