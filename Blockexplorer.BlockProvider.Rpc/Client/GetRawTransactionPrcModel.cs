using System;
using System.Globalization;
using Newtonsoft.Json;

namespace Blockexplorer.BlockProvider.Rpc.Client
{
	public class GetRawTransactionPrcModel
	{
		[JsonProperty("hex")]
		public string Hex { get; set; }

		[JsonProperty("txid")]
		public string Txid { get; set; }

		[JsonProperty("hash")]
		public string Hash { get; set; }

		[JsonProperty("size")]
		public uint Size { get; set; }

		[JsonProperty("vsize")]
		public uint VSize { get; set; }

		[JsonProperty("version")]
		public uint Version { get; set; }

		[JsonProperty("locktime")]
		public uint LockTime { get; set; } 

		[JsonProperty("vin")]
		public VinModel[] Vin { get; set; }

		[JsonProperty("vout")]
		public VoutModel[] Vout { get; set; }

		[JsonProperty("blockhash")]
		public string Blockhash { get; set; }

		[JsonProperty("confirmations")]
		public long Confirmations { get; set; }

		[JsonProperty("time")]
		public uint Time { get; set; }

		[JsonProperty("blocktime")]
		public uint BlockTime { get; set; }



		public DateTime GetTime()
		{
			return Time.FromUnixDateTime();
		}

		public DateTime GetBlockTime()
		{
			return BlockTime.FromUnixDateTime();
		}

		// https://bitcoin.org/en/developer-guide#locktime-and-sequence-number
		// https://en.bitcoin.it/wiki/Protocol_documentation#tx
		public string GetLocktime()
		{
			if (LockTime == 0)
				return "Not locked.";
			
			if (LockTime < 500000000)
			{
				return "Locked, Block: " + LockTime;
			}
			return "Locked, Date: "+ BlockTime.FromUnixDateTime().ToString(CultureInfo.CurrentUICulture);
		}

		public class VinModel
		{
			#region case coinbase
			/*
				{
					"coinbase": "04ba760e1a028a06",
					"sequence": 4294967295 // 0xFFFFFFFF
				}
			*/

			/// <summary>
			/// Marks the first tx, The data in "coinbase" can be anything; it isn't used. Bitcoin puts the current compact-format target and the arbitrary-precision "extraNonce" number there, which increments every time the Nonce field in the block header overflows.
			/// https://en.bitcoin.it/wiki/Transaction#general_format_.28inside_a_block.29_of_each_input_of_a_transaction_-_Txin
			/// </summary>
			[JsonProperty("coinbase")]
			public string Coinbase { get; set; }

			/// <summary>
			/// Normally 0xFFFFFFFF/4294967295; irrelevant unless transaction's lock_time is > 0.
			/// </summary>
			[JsonProperty("sequence")]
			public uint Sequence { get; set; }

			#endregion

			#region outpoint

			/*
			 The previous outpoint being spent. See description of outpoint below.
			 https://bitcoin.org/en/developer-reference#raw-transaction-format
			 The data structure used to refer to a particular transaction output, consisting of a 32-byte TXID 
			 and a 4-byte output index number (vout).

				 */

			/// <summary>
			/// The output transaction’s TXID being used.
			/// </summary>
			[JsonProperty("txid")]
			public string Txid { get; set; }

			/// <summary>
			/// The 4-byte output index number (vout) from the output transaction being used.
			/// </summary>
			[JsonProperty("vout")]
			public uint Vout { get; set; }

			#endregion

			[JsonProperty("scriptSig")]
			public ScriptSigModel ScriptSig { get; set; }


			public class ScriptSigModel
			{
				[JsonProperty("asm")]
				public string Asm { get; set; }

				[JsonProperty("hex")]
				public string Hex { get; set; }
			}

		}

		public class VoutModel
		{
			[JsonProperty("value")]
			public decimal Value { get; set; }
			[JsonProperty("n")]
			public int N { get; set; }
			[JsonProperty("scriptPubKey")]
			public ScriptPubKeyModel ScriptPubKey { get; set; }

			public class ScriptPubKeyModel
			{
				[JsonProperty("addresses")]
				public string[] Addresses { get; set; }
				[JsonProperty("hex")]
				public string Hex { get; set; }
			}
		}
	}
}