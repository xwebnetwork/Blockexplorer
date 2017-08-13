using System.Threading.Tasks;
using Blockexplorer.Core.Domain;

namespace Blockexplorer.Core.Repositories
{
    public interface ITransactionRepository : IRepository<Transaction>
    {
        Task SaveAsImport(Transaction entity);
        Task<bool> IsImported(string id);
        Task SetAsImported(string id);
    }
}