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
		readonly BitcoinRpcClient _client;

		public TransactionAdapter(IOptions<RpcSettings> rpcSettings)
		{
			_client = new BitcoinRpcClient(rpcSettings.Value);
		}

		public async Task<Transaction> GetTransaction(string id)
		{
			var tx = await _client.GetRawTransactionAsync(id);
			if (tx == null)
				return null;
			var transaction = new Transaction
			{
				Blockhash = tx.Blockhash,
				TransactionId = tx.Txid,
				Size = tx.Size,
				TransactionIn = new List<In>(),
				TransactionsOut = new List<Out>(),
			};

			// this fails b/c no input validation
			// Debug.Assert(id == transaction.TransactionId);

			int index = 0;
			foreach (var rpcIn in tx.Vin)
			{
				var inp = new In { Index = index };

				if (rpcIn.Coinbase == null)
                {
                    // Retrieve the origin address by retrieving the previous transaction and extracting the receive address
                    var previousTx = await _client.GetRawTransactionAsync(rpcIn.Txid);

                    if (previousTx.Vout.Length == 1)
                    {
                        // Get the only address, as there is no change address created (full wallet amount was sent)
                        inp.Address = previousTx.Vout[0].ScriptPubKey.Addresses.First();
                    }
                    else
                    {
                        // Pick the second output as the first is the "change" address created by Qt under the hood
                        inp.Address = previousTx.Vout[1].ScriptPubKey.Addresses.First();
                    }
                    
					inp.TransactionId = rpcIn.Txid;
					inp.VOut = (int)rpcIn.Vout;
					inp.Sequence = rpcIn.Sequence;
					inp.ScriptSigHex = rpcIn.ScriptSig.Hex;
				}
				else
				{
					inp.Coinbase = rpcIn.Coinbase;
					inp.Sequence = rpcIn.Sequence;
				}

				transaction.TransactionIn.Add(inp);
			}

			if (transaction.TransactionIn[0].Coinbase != null)
			{
				transaction.IsCoinBase = true;
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
					Index = index++
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
						BitcoinPubKeyAddress address = pubKey.GetAddress(NetworkSpec.ObsidianMain());
						@out.Address = address.ToString();
					}
					else
					{
						@out.Address = "none";
					}

				}

				transaction.TransactionsOut.Add(@out);
			}



			return transaction;
		}
	}
}
