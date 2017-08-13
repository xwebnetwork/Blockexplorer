using System.Threading.Tasks;
using Blockexplorer.Core.Domain;

namespace Blockexplorer.Core.Interfaces
{
    public interface IBlockProvider
    {
        Task<Block> GetBlock(string id);
        Task<Block> GetLastBlock();
    }
}
