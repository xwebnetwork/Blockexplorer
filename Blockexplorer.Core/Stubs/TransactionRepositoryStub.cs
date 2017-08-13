using System.Threading.Tasks;
using Blockexplorer.Core.Domain;
using Blockexplorer.Core.Repositories;
using Microsoft.Extensions.Logging;

namespace Blockexplorer.Core.Stubs
{
	public class TransactionRepositoryStub : ITransactionRepository
	{
		readonly ILogger _log;

		public TransactionRepositoryStub(ILoggerFactory loggerfactory)
		{
			_log = loggerfactory.CreateLogger(GetType());
		}

		public async Task<Transaction> GetById(string id)
		{
			_log.LogWarning("Not implemented!");
			return null;
		}

		public async Task<bool> IsImported(string id)
		{
			_log.LogWarning("Not implemented!");
			return false;
		}

		public async Task Save(Transaction entity)
		{
			_log.LogWarning("Not implemented!");
		}

		public async Task SaveAsImport(Transaction entity)
		{
			_log.LogWarning("Not implemented!");
		}

		public async Task SetAsImported(string id)
		{
			_log.LogWarning("Not implemented!");
		}
	}
}
