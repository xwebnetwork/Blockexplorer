using System.Threading.Tasks;
using Blockexplorer.Core.Domain;
using Blockexplorer.Core.Repositories;
using Microsoft.Extensions.Logging;

namespace Blockexplorer.Core.Stubs
{
	public class BlockRepositoryStub : IBlockRepository
	{
		readonly ILogger _log;

		public BlockRepositoryStub(ILoggerFactory loggerfactory)
		{
			_log = loggerfactory.CreateLogger(GetType());
		}

		public async Task<Block> GetById(string id)
		{
			_log.LogWarning("Not implemented!");
			return null;
		}

		public async Task<bool> IsImported(string id)
		{
			_log.LogWarning("Not implemented!");
			return false;
		}

		public async Task Save(Block entity)
		{
			_log.LogWarning("Not implemented!");
		}

		public async Task SaveAsImport(Block entity)
		{
			_log.LogWarning("Not implemented!");
		}

		public async Task SetAsImported(string id)
		{
			_log.LogWarning("Not implemented!");
		}
	}
}
