using Blockexplorer.Core.Repositories;
using System;
using Blockexplorer.Core.Domain;
using System.Threading.Tasks;
using Blockexplorer.Entities;
using System.Collections.Generic;

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
				var addressEntity = await  db.AddressEntities.FindAsync(id);
				if (addressEntity== null)
					return null;

				List<Transaction> transactions = new List<Transaction>();
				string[] txids = addressEntity.TxIdBlob.Split("\r\n");
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

		public async Task Save(Address entity)
		{

		}

		public async Task UpdateAddress(Address address)
		{

		}
	}
}
