using Newtonsoft.Json;

namespace Blockexplorer.BlockProvider.Rpc.Client
{
	public class GetInfoRpcModel
	{
		[JsonProperty("version")]
		public string Version { get; set; }

		[JsonProperty("blocks")]
		public int Blocks { get; set; }

		[JsonProperty("moneysupply")]
		public decimal MoneySupply { get; set; }

		[JsonProperty("connections")]
		public int Connections { get; set; }

		[JsonProperty("errors")]
		public string Errors { get; set; }
	}
}