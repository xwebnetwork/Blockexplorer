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

		public async Task Save(Address entity)
		{
			_log.LogWarning("Not implemented!");
		}

		public async Task UpdateAddress(Address address)
		{
			_log.LogWarning("Not implemented!");
		}
	}
}
