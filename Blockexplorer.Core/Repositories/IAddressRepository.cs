using System.Threading.Tasks;
using Blockexplorer.Core.Domain;

namespace Blockexplorer.Core.Repositories
{
    public interface IAddressRepository : IRepository<Address>
    {
        Task UpdateAddress(Address address); 
    }
}
