using System.Threading.Tasks;
using Blockexplorer.Core.Domain;
using Blockexplorer.Core.Repositories;

namespace Blockexplorer.Core.Interfaces.Services
{
    public interface IBlockService
    {
        IBlockchainDataProvider BlockchainDataProvider { get; set; }
        IBlockRepository BlockRepository { get; set; }

        Task<Block> GetBlock(string id);
        Task<Block> GetLastBlock(); 
    }
}
