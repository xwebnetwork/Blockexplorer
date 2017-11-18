using System;
using System.Threading.Tasks;
using Blockexplorer.Core.Enums;
using Blockexplorer.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Blockexplorer.Services
{
	public class SearchService : ISearchService
	{
		readonly IBlockService _blockService;
		readonly ITransactionService _transactionService;
		readonly IAddressService _addressService;
		readonly ILogger _log;

		public SearchService(IBlockService blockService,
							 ITransactionService transactionService,
							 IAddressService addressService,
							 ILoggerFactory loggerFactory)
		{
			_blockService = blockService;
			_transactionService = transactionService;
			_addressService = addressService;
			_log = loggerFactory.CreateLogger(GetType());
		}

		public async Task<EntitySearchResult> SearchEntityById(string id)
		{
			try
			{
				id = id.Trim();

				int blockHeight;
				if (int.TryParse(id, out blockHeight) || id.Length == 64)
				{
					Core.Domain.Block block = await _blockService.GetBlock(id);

					if (block != null)
					{
						return EntitySearchResult.Block;
					}
				}
					
				if(id.Length == 64)
				{
					var transaction = await _transactionService.GetTransaction(id);

					if (transaction != null)
					{
						return EntitySearchResult.Transaction;
					}
				}
				
				if(id.Length == 34)
				{
					var address = await _addressService.GetAddress(id);

					if (address != null)
					{
						return EntitySearchResult.Address;
					}
				}

			}
			catch (Exception ex)
			{
				_log.LogError(ex.Message);
			}

			return EntitySearchResult.NotFound;
		}
	}
}