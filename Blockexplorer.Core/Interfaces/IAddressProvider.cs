using System.Threading.Tasks;
using Blockexplorer.Core.Domain;

namespace Blockexplorer.Core.Interfaces
{
    public interface IAddressProvider
    {
        Task<Address> GetAddress(string id);
    }
}
