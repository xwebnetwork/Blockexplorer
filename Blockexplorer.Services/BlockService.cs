using System;
using System.Threading.Tasks;
using Blockexplorer.Core.Domain;
using Blockexplorer.Core.Interfaces;
using Blockexplorer.Core.Interfaces.Services;
using Blockexplorer.Core.Repositories;
using Microsoft.Extensions.Logging;

namespace Blockexplorer.Services
{
    public class BlockService : IBlockService
    {
	    readonly ILogger _log;

		public IBlockchainDataProvider BlockchainDataProvider { get; set; }
        public IBlockRepository BlockRepository { get; set; }
	   

        public BlockService(IBlockchainDataProvider blockchainProvider,
                            IBlockRepository blockRepository,
                            ILoggerFactory loggerFactory)
        {
            BlockchainDataProvider = blockchainProvider;
            BlockRepository = blockRepository;
            _log = loggerFactory.CreateLogger(GetType());
        } 

        public async Task<Block> GetBlock(string id)
        {
            Block block;

            try
            {
                block = await BlockRepository.GetById(id);

                if(block == null) 
                { 
                    block = await BlockchainDataProvider.GetBlock(id);

                    if(block != null)
                    {
                        await BlockRepository.Save(block);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                return null;
            }

            return block;
        }

        public async Task<Block> GetLastBlock()
        {
            Block block;

            try
            {
                block = await BlockchainDataProvider.GetLastBlock();
            }
            catch(Exception ex)
            {
				_log.LogError(ex.Message);
				return null;
            }

            return block;
        }
    } 
}