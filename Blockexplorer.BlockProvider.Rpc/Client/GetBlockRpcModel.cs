using System;
using Newtonsoft.Json;

namespace Blockexplorer.BlockProvider.Rpc.Client
{
    public static class DataTimeUtils
    {
        private static DateTime _baseDateTime = new DateTime(1970, 1, 1);

        public static DateTime FromUnixDateTime(this uint unixDateTime)
        {
            return _baseDateTime.AddSeconds(unixDateTime);
        }
    }


    public class GetBlockRpcModel : RpcModelBase
    {
		/// <summary>
	    /// The block' hash.
	    /// </summary>
	    [JsonProperty("hash")]
	    public string Hash { get; set; }

	    /// <summary>
	    /// The number of confirmations the transactions in this block have, starting at 1 when this block is at the tip of 
	    /// the best block chain. This score will be -1 if the the block is not part of the best block chain.
	    /// </summary>
	    [JsonProperty("confirmations")]
	    public int Confirmations { get; set; }

	    /// <summary>
	    /// The size of this block in serialized block format, counted in bytes.
	    /// </summary>
	    [JsonProperty("size")]
	    public uint Size { get; set; }

	    /// <summary>
	    /// Added in Bitcoin Core 0.13.0
	    /// The size of this block in serialized block format excluding witness data, counted in bytes.
	    /// </summary>
	    [JsonProperty("strippedsize")]
	    public uint StrippedSize { get; set; }

	    /// <summary>
	    /// Added in Bitcoin Core 0.13.0
	    /// This block’s weight as defined in BIP141.
	    /// </summary>
	    [JsonProperty("weight")]
	    public uint Weight { get; set; }

	    /// <summary>
	    /// The height of this block on its block chain.
	    /// </summary>
	    [JsonProperty("height")]
	    public uint Height { get; set; }

	    /// <summary>
	    /// The blocks version number.
	    /// </summary>
	    [JsonProperty("version")]
	    public uint Version { get; set; }

	    /// <summary>
	    /// Added in Bitcoin Core 0.13.0
	    /// This block’s version formatted in hexadecimal
	    /// </summary>
	    [JsonProperty("versionHex")]
	    public string VersionHex { get; set; }

	    /// <summary>
	    /// The merkle root for this block encoded as hex in RPC byte order.
	    /// </summary>
	    [JsonProperty("merkleroot")]
	    public string MerkleRoot { get; set; }

	    /// <summary>
	    /// An array containing the TXIDs of all transactions in this block. 
	    /// The transactions appear in the array in the same order they appear in the serialized block.
	    /// </summary>
	    [JsonProperty("tx")]
	    public string[] Tx { get; set; }

	    /// <summary>
	    /// The block time in seconds since epoch (Jan 1 1970 GMT).
	    /// </summary>
	    [JsonProperty("time")]
	    public uint Time { get; set; }

	    /// <summary>
	    /// Added in Bitcoin Core 0.12.0
	    /// The median block time in Unix epoch time
	    /// </summary>
	    [JsonProperty("mediantime")]
	    public uint MedianTime { get; set; }

	    /// <summary>
	    /// The nonce which was successful at turning this particular block
	    /// into one that could be added to the best block chain.
	    /// </summary>
	    [JsonProperty("nonce")]
	    public uint Nonce { get; set; }

	    /// <summary>
	    /// The target threshold this block's header had to pass.
	    /// </summary>
	    [JsonProperty("bits")]
	    public string Bits { get; set; }

	    /// <summary>
	    /// The estimated amount of work done to find this block relative to the estimated amount of work done to find block 0.
	    /// </summary>
	    [JsonProperty("difficulty")]
	    public double Difficulty { get; set; }

	    /// <summary>
	    /// The estimated number of block header hashes miners had to check from the genesis block to this block, encoded as big-endian hex.
	    /// </summary>
	    [JsonProperty("chainwork")]
	    public string Chainwork { get; set; }

	    /// <summary>
	    /// The hash of the header of the previous block,
	    /// encoded as hex in RPC byte order.
	    /// </summary>
	    [JsonProperty("previousblockhash")]
	    public string PreviousBlockHash { get; set; }

	    /// <summary>
	    /// The hash of the header of the previous block,
	    /// encoded as hex in RPC byte order.
	    /// </summary>
	    [JsonProperty("nextblockhash")]
	    public string NextBlockHash { get; set; }

        /// <summary>
	    /// The flag on the block
	    /// </summary>
	    [JsonProperty("flags")]
        public string Flags { get; set; }

        //DateTime Time => GetTime();

        public DateTime GetTime()
        {
            return Time.FromUnixDateTime();
        }

        public bool IsLastBlock()
        {
            return string.IsNullOrEmpty(NextBlockHash);
        }
    }
}
