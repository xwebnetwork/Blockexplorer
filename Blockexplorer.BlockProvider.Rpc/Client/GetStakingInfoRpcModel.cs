using Newtonsoft.Json;

namespace Blockexplorer.BlockProvider.Rpc.Client
{
    public class GetStakingInfoRpcModel
    {
        [JsonProperty("enabled")]
        public bool Enabled { get; set; }

        [JsonProperty("staking")]
        public bool Staking { get; set; }

        [JsonProperty("errors")]
        public string Errors { get; set; }

        [JsonProperty("currentblocksize")]
        public int CurrentBlockSize { get; set; }

        [JsonProperty("currentblocktx")]
        public int CurrentBlockTx { get; set; }

        [JsonProperty("pooledtx")]
        public int PooledTx { get; set; }

        [JsonProperty("difficulty")]
        public double Difficulty { get; set; }

        [JsonProperty("search-interval")]
        public int SearchInterval { get; set; }

        [JsonProperty("weight")]
        public long Weight { get; set; }

        [JsonProperty("netstakeweight")]
        public long NetStakeWeight { get; set; }

        [JsonProperty("expectedtime")]
        public int ExpectedTime { get; set; }
    }
}