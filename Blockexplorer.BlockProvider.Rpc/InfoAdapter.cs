using System.Threading.Tasks;
using Blockexplorer.BlockProvider.Rpc.Client;
using Blockexplorer.Core.Domain;
using Blockexplorer.Core.Interfaces;
using Microsoft.Extensions.Options;

namespace Blockexplorer.BlockProvider.Rpc
{
	public class InfoAdapter : IInfoAdapter
	{
		readonly BitcoinRpcClient _client;

		public InfoAdapter(IOptions<RpcSettings> rpcSettings)
		{
			_client = new BitcoinRpcClient(rpcSettings.Value);
		}

		public async Task<Info> GetInfo()
		{
			var rpcInfo = await _client.GetInfo();
			var info = new Info
			{
			  Version = rpcInfo.Version,
			  Blocks = rpcInfo.Blocks,
			  Connections = rpcInfo.Connections,
			  Errors = rpcInfo.Errors,
			  MoneySupply = rpcInfo.MoneySupply
			};
			return info;
		}
	}
}
