using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Blockexplorer.BlockProvider.Rpc.Client;
using Blockexplorer.Core.Domain;
using Blockexplorer.Core.Interfaces;
using Microsoft.Extensions.Options;
using NBitcoin;
using NBitcoin.DataEncoders;
using Transaction = Blockexplorer.Core.Domain.Transaction;


namespace Blockexplorer.BlockProvider.Rpc
{
	public class TransactionAdapter : ITransactionProvider
	{
		public const string NonStandardAddress = "nonstandard";
		public readonly BitcoinRpcClient RpcClient;

		public TransactionAdapter(IOptions<RpcSettings> rpcSettings)
		{
			RpcClient = new BitcoinRpcClient(rpcSettings.Value);
		}

		public async Task<Transaction> GetTransaction(string id)
		{
			try
			{


				GetRawTransactionRpcModel tx = await RpcClient.GetRawTransactionAsync(id);
				if (tx == null)
					return null;

				TransactionType transactiontype = GetTransactionType(tx);

				var transaction = new Transaction
				{
					OriginalJson = tx.OriginalJson,
					TransactionType = transactiontype,
					Blockhash = tx.Blockhash,
					TransactionId = tx.Txid,
					Size = tx.Size,
					TransactionIn = new List<VIn>(),
					TransactionsOut = new List<Out>(),
					Time = tx.GetTime()
				};


				int index = 0;
				foreach (var rpcIn in tx.Vin)
				{
					var vIn = new VIn
					{
						Index = index,
						Coinbase = rpcIn.Coinbase,
						Sequence = rpcIn.Sequence,
						ScriptSigHex = rpcIn.ScriptSig?.Hex,
						AssetId = null,
						// pointer to previous tx/vout:
						PrevTxIdPointer = rpcIn.Txid,
						PrevVOutPointer = (int)rpcIn.Vout,
						// we'll try to fetch this id possible
						PrevVOutFetchedAddress = null,
						PrevVOutFetchedValue = 0
					};

					if (rpcIn.Txid != null)
					{
						// Retrieve the origin address by retrieving the previous transaction and extracting the receive address and value
						var previousTx = await RpcClient.GetRawTransactionAsync(rpcIn.Txid);
						if (previousTx != null)
						{
							var n = rpcIn.Vout;
							Debug.Assert(n == previousTx.Vout[n].N);
							vIn.PrevVOutFetchedAddress = previousTx.Vout[n].ScriptPubKey.Addresses.First();
							vIn.PrevVOutFetchedValue = previousTx.Vout[n].Value;
						}
					}
					transaction.TransactionIn.Add(vIn);
				}



				index = 0;
				foreach (var output in tx.Vout)
				{
					var @out = new Out
					{
						TransactionId = transaction.TransactionId,
						Value = output.Value,
						Quantity = output.N,
						AssetId = null,
						Index = index++,
					};

					if (output.ScriptPubKey.Addresses != null) // Satoshi 14.2
						@out.Address = output.ScriptPubKey.Addresses.FirstOrDefault();
					else
					{
						string hexScript = output.ScriptPubKey.Hex;

						if (!string.IsNullOrEmpty(hexScript))
						{
							byte[] decodedScript = Encoders.Hex.DecodeData(hexScript);
							Script script = new Script(decodedScript);
							var pubKey = PayToPubkeyTemplate.Instance.ExtractScriptPubKeyParameters(script);
							if (pubKey != null)
							{
								BitcoinPubKeyAddress address = pubKey.GetAddress(NetworkSpec.ObsidianMain());
								@out.Address = address.ToString();
							}
							else
							{
								@out.Address = script.ToString();
							}

						}
						else
						{
							Debug.Assert(output.ScriptPubKey.Type == NonStandardAddress);
							@out.Address = output.ScriptPubKey.Type;
						}
					}
					transaction.TransactionsOut.Add(@out);
				}

				return transaction;
			}
			catch { }
			return null;
		}

		static TransactionType GetTransactionType(GetRawTransactionRpcModel rpctx)
		{
			if (rpctx.Vin[0].Coinbase != null)
				return TransactionType.PoW_Reward_Coinbase;

			var vout_0 = rpctx.Vout[0];
			if (rpctx.Vout.Length == 3
				&& vout_0.N == 0
				&& vout_0.ScriptPubKey.Type == NonStandardAddress
				&& string.IsNullOrEmpty(vout_0.ScriptPubKey.Hex))
				return TransactionType.PoS_Reward;

			return TransactionType.Money;
		}
	}
}
