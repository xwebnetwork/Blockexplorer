using System.Threading.Tasks;
using Blockexplorer.Core.Domain;
using System.Collections.Generic;

namespace Blockexplorer.Core.Interfaces.Services
{
    public interface IAddressService
    {
        Task<Address> GetAddress(string id);
		Task<List<Address>> GetTopList();

	}
}
