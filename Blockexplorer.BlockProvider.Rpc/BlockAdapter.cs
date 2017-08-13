using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blockexplorer.BlockProvider.Rpc.Client;
using Blockexplorer.Core.Domain;
using Blockexplorer.Core.Interfaces;
using Microsoft.Extensions.Options;

namespace Blockexplorer.BlockProvider.Rpc
{
    public class BlockAdapter : IBlockProvider
    {
        readonly BitcoinRpcClient _client;

        public BlockAdapter(IOptions<RpcSettings> rpcSettings)
        {
            _client = new BitcoinRpcClient(rpcSettings.Value);
        }

        public async Task<Block> GetBlock(string id)
        {
	        if (id == null)
		        return null;

	        string input = id.Trim();
	        if (input.Length == 0 || input.Length > 64)
		        return null;

	        try
	        {
		        string blockHash = null;
		        uint blocknumber;
		        if (input.Length != 64 && uint.TryParse(input, out blocknumber))
		        {
			        try
			        {
				        blockHash = await _client.GetBlockHashAsync(blocknumber);
				        if (blockHash == null)
					        return null;
			        }
			        catch (Exception e)
			        {
				        return null;
			        }
		        }
		        else
		        {
			        blockHash = input;
				}

				

		        GetBlockRpcModel b = await _client.GetBlockAsync(blockHash);
		        if (b == null)
			        return null;

				if(b.Tx == null)
					b.Tx = new string[0];

		        var block = new Block
		        {
			        Hash = b.Hash,
			        Height = b.Height,
			        Time = b.GetTime(),
			        Difficulty = b.Difficulty,
			        MerkleRoot = b.MerkleRoot,
			        Nonce = b.Nonce,
			        PreviousBlock = b.PreviousBlockHash,
					NextBlock = b.NextBlockHash,
			        Confirmations = b.Confirmations,
			        TotalTransactions = b.Tx.Length,
			        Transactions = new List<Transaction>(b.Tx.Length),
					Chainwork = b.Chainwork,
					Bits = b.Bits,
					Size = (int)b.Size,
					StrippedSize = (int)b.StrippedSize,
					VersionHex = b.VersionHex,
					Version = b.Version,
					Weight = b.Weight

		        };

		        foreach (var t in b.Tx)
		        {
			        var transaction = new Transaction
			        {
				        TransactionId = t,
				        Blockhash = block.Hash,
				        Block = block,
				        Confirmations = block.Confirmations,
				        Time = block.Time
			        };
			        block.Transactions.Add(transaction);
		        }

		        return block;
	        }
	        catch (Exception e)
	        {
		        ;
		        return null;
	        }
        }

        public async Task<Block> GetLastBlock()
        {
	        var blockhash = await _client.GetBestBlockHash();
	        var block =  await GetBlock(blockhash) ?? new Block {Hash = blockhash};
	        return block;
        }
    }
}  
