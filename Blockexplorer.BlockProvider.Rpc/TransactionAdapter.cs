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

			Debug.Assert(id == transaction.TransactionId);

			int index = 0;
			foreach (var rpcIn in tx.Vin)
			{
				var inp = new In { Index = index };

				if (rpcIn.Coinbase == null)
				{
					string hexScript = rpcIn.ScriptSig.Hex;
					byte[] decodedScript = Encoders.Hex.DecodeData(hexScript);
					Script script = new Script(decodedScript);
					PayToPubkeyHashTemplate template = new PayToPubkeyHashTemplate();


					PayToPubkeyHashScriptSigParameters param = template.ExtractScriptSigParameters(script);
					if (param != null)
					{
						PubKey pubKey = param.PublicKey;
						BitcoinPubKeyAddress address = pubKey.GetAddress(NetworkSpec.ObsidianMain());

						inp.Address = address.ToString();
					}
					else inp.Address = "none";

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
				Debug.Assert(transaction.TransactionIn.Count == 1);
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
