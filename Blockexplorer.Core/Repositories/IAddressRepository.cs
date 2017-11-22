using System.Threading.Tasks;
using Blockexplorer.Core.Domain;
using System.Collections.Generic;

namespace Blockexplorer.Core.Repositories
{
    public interface IAddressRepository : IRepository<Address>
    {
		Task<List<Address>> GetTopList();

	}
}
