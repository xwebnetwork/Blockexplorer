using System;
using System.Threading;
using System.Threading.Tasks;
using Blockexplorer.BlockProvider.Rpc.Client;
using Blockexplorer.Entities;
using NBitcoin.DataEncoders;
using NBitcoin;
using Blockexplorer.BlockProvider.Rpc;
using System.Diagnostics;
using System.Linq;

namespace Blockexplorer.Indexer
{
	class Program
	{
		static readonly CancellationTokenSource Cts = new CancellationTokenSource();
		static void Main(string[] args)
		{
			Console.WriteLine("Indexer Starting...press any key to cancel.");
			Task.Run(() => Run(Cts.Token));
			Console.ReadLine();
			Console.WriteLine("Cancelling...");
			Cts.Cancel();

		}

		static async Task<int> Run(CancellationToken token)
		{
			var settings = new RpcSettings { User = "me", Password = "123", Url = "http://me:123@127.0.0.1:8332/" };
			var client = new BitcoinRpcClient(settings);
			var db = new ObsidianChainContext();

			uint currentblockHeight = 0;
			if (db.Blocks.Any())
			{
				currentblockHeight = (uint)db.Blocks.Max(x => x.Height) + 1;
			}

			while (!token.IsCancellationRequested)
			{
				try
				{
					getBlock:
					db = new ObsidianChainContext();

					string blockHash = await client.GetBlockHashAsync(currentblockHeight);

					if (blockHash == null)
					{
						Console.WriteLine($"Block {currentblockHeight} not found, waiting...");
						await Task.Delay(1000);
						if (!token.IsCancellationRequested)
							goto getBlock;
						return 0;
					}

					Console.WriteLine($"Processing block {currentblockHeight}: {blockHash}");

					GetBlockRpcModel block = await client.GetBlockAsync(blockHash);
					if (block == null)
						return -1;


					var blockEntity = new BlockEntity { Id = Guid.NewGuid(), Height = (int)currentblockHeight, BlockHash = blockHash, BlockData = block.OriginalJson };
					db.Blocks.Add(blockEntity);

					string[] transactions = block.Tx;
					foreach (var txid in transactions)
					{
						var transactionEntity = new TransactionEntity() { Id = txid, BlockEntity = blockEntity };
						GetRawTransactionRpcModel transaction = null;
						try
						{
							transaction = await client.GetRawTransactionAsync(txid);
							transactionEntity.TransactionData = transaction.OriginalJson;
						}
						catch { }

						db.Transactions.Add(transactionEntity);

						if (transaction == null) // block 0?
							continue;


						int txoutIndex = 0;
						foreach (var output in transaction.Vout)
						{
							Debug.Assert(output.N == txoutIndex);

							string address = null;

							if (output.ScriptPubKey != null && !string.IsNullOrEmpty(output.ScriptPubKey.Hex))
							{
								byte[] decodedScript = Encoders.Hex.DecodeData(output.ScriptPubKey.Hex);
								Script script = new Script(decodedScript);
								var pubKey = PayToPubkeyTemplate.Instance.ExtractScriptPubKeyParameters(script);
								if (pubKey != null)
								{
									BitcoinPubKeyAddress bitcoinPubKeyAddress = pubKey.GetAddress(NetworkSpec.ObsidianMain());
									if (bitcoinPubKeyAddress != null)
										address = bitcoinPubKeyAddress.ToString();
								}
							}

							if(address != null) // it can be null!
							{
								var transactionAddress = new TransactionAddressEntity
								{
									Address = address,
									TransactionEntity = transactionEntity,
								};
								db.TransactionAddresses.Add(transactionAddress);
							}
							

							txoutIndex++;
						}
					}

					await db.SaveChangesAsync();
					currentblockHeight++;
				}
				catch (Exception e)
				{
					Console.WriteLine(e.Message);
				}

			}

			return 0;
		}
	}
}
