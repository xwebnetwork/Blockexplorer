using System.Threading.Tasks;
using Blockexplorer.Core.Domain;
using Blockexplorer.Core.Repositories;

namespace Blockexplorer.Core.Interfaces.Services
{
    public interface IAddressService
    {
        IBlockchainDataProvider BlockchainDataProvider { get; set; }
        IBlockRepository BlockRepository { get; set; } 

        Task<Address> GetAddress(string id);
    }
}
