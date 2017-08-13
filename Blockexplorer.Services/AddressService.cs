using System;
using System.Threading.Tasks;
using Blockexplorer.Core.Domain;
using Blockexplorer.Core.Interfaces;
using Blockexplorer.Core.Interfaces.Services;
using Blockexplorer.Core.Repositories;
using Microsoft.Extensions.Logging;

namespace Blockexplorer.Services
{
    public class AddressService : IAddressService
    {
        public IBlockchainDataProvider BlockchainDataProvider { get; set; }
        public IBlockRepository BlockRepository { get; set; }
        public IAddressRepository AddressRepository { get; set; }
	    readonly ILogger _log;

        public AddressService(IBlockchainDataProvider blockchainProvider,
                            IBlockRepository blockRepository,
                            IAddressRepository addressRepository,
	        ILoggerFactory loggerFactory)
        {
            BlockchainDataProvider = blockchainProvider; 
            BlockRepository = blockRepository;
            AddressRepository = addressRepository;
            _log = loggerFactory.CreateLogger(GetType());
        }
         
        public async Task<Address> GetAddress(string id)
        { 
            Address address;

            try
            {
                address = await BlockchainDataProvider.GetAddress(id);

                if(address != null)
                {
                    await AddressRepository.UpdateAddress(address);
                }
            }
            catch(Exception ex)
            {
				_log.LogError(ex.Message);
				return null;
            }

            return address;
        }
    }
} 