using System;
using System.Threading;
using System.Threading.Tasks;
using Blockexplorer.Core.Domain;
using Blockexplorer.Core.Interfaces;
using Blockexplorer.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Blockexplorer.Services
{
	public class ApiService : IApiService
	{
		readonly IBlockchainDataProvider _blockchainProvider;
		readonly ILogger _log;

		static DateTime _lastQuery = DateTime.MinValue;
		static Info _info = new Info { Errors = "Not Initialzed" };
		static readonly SemaphoreSlim Sem = new SemaphoreSlim(1, 1);

		public ApiService(IBlockchainDataProvider blockchainProvider, ILoggerFactory loggerFactory)
		{
			_blockchainProvider = blockchainProvider;
			_log = loggerFactory.CreateLogger(GetType());
		}

		public async Task<Info> GetInfo()
		{
			if (UseCachedInfo())
				return _info;

			await Sem.WaitAsync();
			try
			{
				_info = await _blockchainProvider.GetInfo();
				_lastQuery = DateTime.UtcNow;
				return _info;
			}
			catch (Exception e)
			{
				_log.LogError(e.Message);
				return new Info { Errors = "Exception" };
			}
			finally
			{
				Sem.Release();
			}
		}

		static bool UseCachedInfo()
		{
			var now = DateTime.UtcNow;
			var last = _lastQuery;
			TimeSpan elapsed = now - last;
			if (elapsed.TotalSeconds > 60)
				return false;
			return true;
		}
	}
}
