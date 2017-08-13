using System.Threading.Tasks;
using Blockexplorer.Core.Domain;
using Blockexplorer.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Blockexplorer.BlockProvider
{
    public class ChainProvider : IBlockchainDataProvider
    {
	    readonly ITransactionProvider _transactionProvider;
	    readonly IBlockProvider _blockProvider;
	    readonly IAddressProvider _addressProvider;
	    readonly ILogger _log;

        public ChainProvider(ITransactionProvider transactionProvider,
                                  IBlockProvider blockProvider,
                                  IAddressProvider addressProvider,
                                  ILoggerFactory loggerFactory)
        {
            _transactionProvider = transactionProvider;
            _blockProvider = blockProvider;
            _addressProvider = addressProvider;
            _log = loggerFactory.CreateLogger(GetType());
        }

        public async Task<Block> GetBlock(string id)
        {
            return await _blockProvider.GetBlock(id); 
        }

        public async Task<Transaction> GetTransaction(string id)
        {
            return await _transactionProvider.GetTransaction(id);
        }

        public async Task<Block> GetLastBlock()
        {
            return await _blockProvider.GetLastBlock();
        }

        public async Task<Address> GetAddress(string id)
        {
            return await _addressProvider.GetAddress(id);
        }
       
    }
}