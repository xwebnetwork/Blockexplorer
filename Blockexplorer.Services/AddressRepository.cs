using Blockexplorer.Core.Repositories;
using System.Linq;
using Blockexplorer.Core.Domain;
using System.Threading.Tasks;
using Blockexplorer.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Blockexplorer.Services
{
	public class AddressRepository : IAddressRepository
	{
		public async Task<Address> GetById(string id)
		{
			using (var db = new ObsidianChainContext())
			{
				var transactionAddresses = db.TransactionAddresses.Where(x => x.Address == id).ToList();
				if (transactionAddresses.Count == 0)
					return null;

				List<Transaction> transactions = new List<Transaction>();
				foreach (var txadr in transactionAddresses)
				{
					var transaction = new Transaction
					{
						TransactionId = txadr.TransactionEntityId
					};
					transactions.Add(transaction);
					
				}

				var address = new Address
				{
					UncoloredAddress = id,
					Transactions = transactions,
					TotalTransactions = transactions.Count
				};
				return address;
			}
		}

		public async Task Save(Address entity)
		{

		}

		public async Task UpdateAddress(Address address)
		{

		}
	}
}
