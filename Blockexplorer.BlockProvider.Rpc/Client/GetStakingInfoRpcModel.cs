using Newtonsoft.Json;

namespace Blockexplorer.BlockProvider.Rpc.Client
{
    public class GetStakingInfoRpcModel
    {
        [JsonProperty("currentblocksize")]
            public int CurrentBlockSize { get; set; }

        [JsonProperty("currentblocktx")]
            public int CurrentBlockTx { get; set; }

        [JsonProperty("pooledtx")]
            public int PooledTx { get; set; }

        [JsonProperty("difficulty")]
            public decimal Difficulty { get; set; }

        [JsonProperty("search-interval")]
            public int SearchInterval { get; set; }

        [JsonProperty("weight")]
            public int Weight { get; set; }

        [JsonProperty("netstakeweight")]
            public int NetStakeWeight { get; set; }

        [JsonProperty("expectedtime")]
            public int ExpectedTime { get; set; }
    }
}
