using System.Threading.Tasks;
using Blockexplorer.Core.Domain;

namespace Blockexplorer.Core.Repositories
{
    public interface IBlockRepository : IRepository<Block>
    {
        Task SaveAsImport(Block entity);
        Task<bool> IsImported(string id);
        Task SetAsImported(string id);
    }
}