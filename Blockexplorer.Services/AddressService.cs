using System;
using System.Threading.Tasks;
using Blockexplorer.Core.Domain;
using Blockexplorer.Core.Interfaces.Services;
using Blockexplorer.Core.Repositories;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Blockexplorer.Services
{
    public class AddressService : IAddressService
    {
		readonly IAddressRepository _addressRepository;
	    readonly ILogger _log;

        public AddressService(IAddressRepository addressRepository, ILoggerFactory loggerFactory)
        {
            _addressRepository = addressRepository;
            _log = loggerFactory.CreateLogger(GetType());
        }
         
        public async Task<Address> GetAddress(string id)
        { 
            Address address;

            try
            {
                address = await _addressRepository.GetById(id);
            }
            catch(Exception ex)
            {
				_log.LogError(ex.Message);
				return null;
            }

            return address;
        }

		
		public async Task<List<Address>> GetTopList()
		{
			List<Address> addresses;

			try
			{
				addresses = await _addressRepository.GetTopList();
				return addresses;
			}
			catch (Exception ex)
			{
				_log.LogError(ex.Message);
				throw;
			}
		}
	}
} 