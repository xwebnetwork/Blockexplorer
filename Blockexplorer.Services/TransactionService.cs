using System;
using System.Threading.Tasks;
using Blockexplorer.Core.Domain;
using Blockexplorer.Core.Interfaces;
using Blockexplorer.Core.Interfaces.Services;
using Blockexplorer.Core.Repositories;
using Microsoft.Extensions.Logging;

namespace Blockexplorer.Services
{
    public class TransactionService : ITransactionService
    {
	    readonly ILogger _log;
		public IBlockchainDataProvider BlockchainDataProvider { get; set; }
        public ITransactionRepository TransactionRepository { get; set; }
        public IBlockService BlockService { get; set; }
	    

        public TransactionService(IBlockchainDataProvider blockchainProvider,
                            ITransactionRepository transactionRepository,
                            IBlockService blockService,
                            ILoggerFactory loggerFactory)
        {
            BlockchainDataProvider = blockchainProvider;
            TransactionRepository = transactionRepository;
            BlockService = blockService;
            _log = loggerFactory.CreateLogger(GetType()); 
        }

        public async Task<Transaction> GetTransaction(string id)
        {
            Transaction transaction;

            try 
            {
                transaction = await TransactionRepository.GetById(id);

                if(transaction == null)
                {
                    transaction = await BlockchainDataProvider.GetTransaction(id);

	                if (transaction == null)
		                return null;

                        transaction.Block = await BlockService.GetBlock(transaction.Blockhash);

                        await TransactionRepository.Save(transaction);

	                foreach (var @in in transaction.TransactionIn)
	                {
						if(@in.PrevTxIdPointer == null)
							continue;
		                var inputTransaction = await BlockchainDataProvider.GetTransaction(@in.PrevTxIdPointer);
		                @in.PrevVOutFetchedValue = inputTransaction.TransactionsOut[@in.PrevVOutPointer].Value;
	                }
                }
            }
            catch (Exception ex)
            {
	            _log.LogError(ex.Message);
                return null;
            }
             
            return transaction;
        }
    }
}