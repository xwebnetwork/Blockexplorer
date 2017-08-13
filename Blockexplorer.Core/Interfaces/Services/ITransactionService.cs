using System.Threading.Tasks;
using Blockexplorer.Core.Domain;
using Blockexplorer.Core.Repositories;

namespace Blockexplorer.Core.Interfaces.Services
{
    public interface ITransactionService
    {
        IBlockchainDataProvider BlockchainDataProvider { get; set; }
        ITransactionRepository TransactionRepository { get; set; }
        IBlockService BlockService { get; set; }
        Task<Transaction> GetTransaction(string id);
    }
}
