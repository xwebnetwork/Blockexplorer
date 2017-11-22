using System;
using System.Threading;
using System.Threading.Tasks;
using Blockexplorer.BlockProvider.Rpc.Client;
using Blockexplorer.Entities;
using Blockexplorer.BlockProvider.Rpc;
using System.Diagnostics;
using System.Linq;
using Blockexplorer.Core.Domain;
using System.Collections.Generic;

namespace Blockexplorer.Indexer
{
	class Program
	{
		const string CRLF = "\r\n";

		static readonly CancellationTokenSource Cts = new CancellationTokenSource();
		static readonly RpcSettings _rpcSettings;
		static readonly TransactionAdapter _txAdapter;

		static int _currentBlockHeight = 0;
		static int _currentBlockNumber = 1;

		static Program()
		{
			_rpcSettings = new RpcSettings { User = "me", Password = "123", Url = "http://me:123@127.0.0.1:8332/" };
			var options = new UserOptions<RpcSettings>() { Value = _rpcSettings };
			_txAdapter = new TransactionAdapter(options);
		}

		static void Main(string[] args)
		{
			Console.WriteLine("Indexer Starting...press any key to cancel.");
			Task.Run(() => Run(Cts.Token));
			Console.ReadLine();
			Console.WriteLine("Cancelling...");
			Cts.Cancel();
		}

		static async Task Run(CancellationToken token)
		{
			while (!token.IsCancellationRequested)
			{
				try
				{
					getBlock:
					using (var db = new ObsidianChainContext())
					{
						if (db.BlockEntities.Any())
						{
							_currentBlockNumber = db.BlockEntities.Max(x => x.Id);
							_currentBlockHeight = db.BlockEntities.Max(x => x.Id) - 1;
							_currentBlockHeight++;
							_currentBlockNumber++;
						}

						string blockHash = await _txAdapter.RpcClient.GetBlockHashAsync(_currentBlockHeight);

						if (blockHash == null)
						{
							Console.WriteLine($"Block at height {_currentBlockHeight} not found, waiting...");
							await Task.Delay(1000);
							if (!token.IsCancellationRequested)
								goto getBlock;
							return;
						}

						var result = await IndexBlock(db, blockHash);
						if (result != 0)
							return;
					}
				}
				catch (Exception e)
				{
					Console.WriteLine(e.ToString());
					Console.WriteLine("Pausing 30 seconds...");
					await Task.Delay(30);
					Console.WriteLine("Resuming...");
				}
			}
		}

		static async Task<int> IndexBlock(ObsidianChainContext db, string blockHash)
		{
			Console.WriteLine($"Processing block at height {_currentBlockHeight}: {blockHash}");

			GetBlockRpcModel block = await _txAdapter.RpcClient.GetBlockAsync(blockHash);
			if (block == null)
			{
				Console.WriteLine($"Error - could not retrieve block not at height {_currentBlockHeight}: {blockHash}");
				return -1;
			}

			var blockEntity = new BlockEntity { Id = _currentBlockNumber, Height = _currentBlockHeight, BlockHash = blockHash, BlockData = block.OriginalJson };
			db.BlockEntities.Add(blockEntity);
			if (_currentBlockHeight == 0)
			{
				// for the tx in the genesis block, we can't pull transaction data, so we make an exception here
				var genesisBlockTransaction = new TransactionEntity { BlockEntity = blockEntity, Id = block.Tx[0] };
				db.TransactionEntities.Add(genesisBlockTransaction);
				_currentBlockHeight++;
				_currentBlockNumber++;
				await db.SaveChangesAsync();
				return 0;
			}

			// Get all transactions from the adapter, so that we can use the logic there
			string[] transactionIds = block.Tx;
			List<Transaction> blockTransactions = new List<Transaction>();
			foreach (var txid in transactionIds)
			{
				var transaction = await _txAdapter.GetTransaction(txid);
				transaction.Block = new Block { Height = _currentBlockHeight };
				blockTransactions.Add(transaction);
			}

			// Queue all for insert to the db
			foreach (var blockTx in blockTransactions)
			{
				var transactionEntity = new TransactionEntity() { Id = blockTx.TransactionId, BlockEntity = blockEntity, TransactionData = blockTx.OriginalJson };
				db.TransactionEntities.Add(transactionEntity);
			}

			// Process all blockTransactions for accounting
			foreach (var blockTx in blockTransactions)
			{
				ProcessTransaction(blockTx, db);
			}

			await db.SaveChangesAsync();
			_currentBlockHeight++;
			_currentBlockNumber++;
			return 0;

		}

		static void ProcessTransaction(Transaction blockTx, ObsidianChainContext db)
		{
			switch (blockTx.TransactionType)
			{
				case TransactionType.PoW_Reward_Coinbase:
					ProcessPoWReward(blockTx, db);
					break;
				case TransactionType.PoS_Reward:
					ProcessStakingReward(blockTx, db);
					break;
				case TransactionType.Money:
					ProcessMoneyTransfer(blockTx, db);
					break;
				default:
					throw new IndexOutOfRangeException("Unsupprted TransactionType.");
			}
		}
		static void ProcessPoWReward(Transaction tx, ObsidianChainContext db)
		{
			var vout = tx.TransactionsOut[0];
			var address = vout.Address;
			var amount = vout.Value;
			AddressEntity existing = db.AddressEntities.Find(address);
			if (existing == null)
			{
				var newAddress = new AddressEntity
				{
					Id = address,
					Balance = amount,
					LastModifiedBlockHeight = (int)tx.Block.Height,
					TxIdBlob = tx.TransactionId + CRLF
				};
				InsertAddressIfShouldBeIndexed(newAddress, db);
				return;
			}
			existing.Balance += amount;
			existing.LastModifiedBlockHeight = (int)tx.Block.Height;
			existing.TxIdBlob += tx.TransactionId + CRLF;
		}

		static void ProcessStakingReward(Transaction tx, ObsidianChainContext db)
		{
			var vin = tx.TransactionIn[0];
			var inAddress = vin.PrevVOutFetchedAddress;
			var oldBalance = vin.PrevVOutFetchedValue;
			Debug.Assert(inAddress == tx.TransactionsOut[1].Address);
			Debug.Assert(inAddress == tx.TransactionsOut[2].Address);
			var outValue1 = tx.TransactionsOut[1].Value;
			var outValue2 = tx.TransactionsOut[2].Value;
			var change = outValue1 + outValue2 - oldBalance;

			// I assume that only pre-existing addresses get a staking reward!
			AddressEntity existing = db.AddressEntities.Find(inAddress);
			Debug.Assert(existing != null);
			existing.Balance += change;
			existing.LastModifiedBlockHeight = (int)tx.Block.Height;
			existing.TxIdBlob += tx.TransactionId + CRLF;
		}


		static void ProcessMoneyTransfer(Transaction tx, ObsidianChainContext db)
		{
			IList<VIn> vins = tx.TransactionIn;
			List<string> inAdresses = new List<string>();
			foreach (var vin in vins)
			{
				AddressEntity existing = db.AddressEntities.Find(vin.PrevVOutFetchedAddress);
				inAdresses.Add(existing.Id);
				existing.Balance -= vin.PrevVOutFetchedValue;
				existing.LastModifiedBlockHeight = (int)tx.Block.Height;
				existing.TxIdBlob += tx.TransactionId + CRLF;
			}
			IList<Out> vouts = tx.TransactionsOut;
			foreach (var vout in vouts)
			{
				string outAdress = vout.Address;
				if (outAdress.StartsWith("OP_RETURN", StringComparison.OrdinalIgnoreCase))
					outAdress = "OP_RETURN";

				AddressEntity existing = db.AddressEntities.Find(outAdress);
				if (existing != null)
				{
					existing.Balance += vout.Value;
					if (!inAdresses.Contains(existing.Id))
					{
						existing.TxIdBlob += tx.TransactionId + CRLF;
						existing.LastModifiedBlockHeight = (int)tx.Block.Height;
					}
				}
				else
				{
					var newAddress = new AddressEntity
					{
						Id = vout.Address,
						Balance = vout.Value,
						LastModifiedBlockHeight = (int)tx.Block.Height,
						TxIdBlob = tx.TransactionId + CRLF
					};
					InsertAddressIfShouldBeIndexed(newAddress, db);
				}
			}
		}

		static void InsertAddressIfShouldBeIndexed(AddressEntity address, ObsidianChainContext db)
		{
			if (address.Id.StartsWith("OP_RETURN", StringComparison.OrdinalIgnoreCase))
				address.Id = "OP_RETURN";
			if (address.Id != TransactionAdapter.NonStandardAddress)
				db.AddressEntities.Add(address);
		}
	}
}
