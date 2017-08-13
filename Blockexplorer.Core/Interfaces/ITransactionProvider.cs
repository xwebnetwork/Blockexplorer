using System.Threading.Tasks;
using Blockexplorer.Core.Domain;

namespace Blockexplorer.Core.Interfaces
{
    public interface ITransactionProvider
    {
        Task<Transaction> GetTransaction(string id);
    }
}