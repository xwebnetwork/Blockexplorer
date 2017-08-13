using System;
using System.Threading.Tasks;
using Blockexplorer.Core.Domain;
using Blockexplorer.Core.Interfaces;

namespace Blockexplorer.BlockProvider.Rpc
{
	public class AddressAdapter : IAddressProvider
	{
		public async Task<Address> GetAddress(string id)
		{
			return null;
		}
	}
}
