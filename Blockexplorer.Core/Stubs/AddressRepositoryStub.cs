using System.Collections.Generic;
using System.Threading.Tasks;
using Blockexplorer.Core.Domain;
using Blockexplorer.Core.Repositories;
using Microsoft.Extensions.Logging;

namespace Blockexplorer.Core.Stubs
{
	public class AddressRepositoryStub : IAddressRepository
	{
		readonly ILogger _log;
		public AddressRepositoryStub(ILoggerFactory loggerfactory)
		{
			_log = loggerfactory.CreateLogger(GetType());
		}

		public async Task<Address> GetById(string id)
		{
			_log.LogWarning("Not implemented!");
			return null;
		}

		public async Task<List<Address>> GetTopList()
		{
			return new List<Address>();
		}

		public async Task Save(Address entity)
		{
			_log.LogWarning("Not implemented!");
		}

		
	}
}
