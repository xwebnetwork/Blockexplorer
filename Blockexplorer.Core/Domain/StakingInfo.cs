namespace Blockexplorer.Core.Domain
{
    /// <summary>
    /// Model returns values from the 'getstakinginfo' RPC call.
    /// </summary>
    public class StakingInfo
    {
        public string Errors { get; set; }
        public int CurrentBlockSize { get; set; }
        public int CurrentBlockTx { get; set; }
        public int PooledTx { get; set; }
        public double Difficulty { get; set; }
        public int SearchInterval { get; set; }
        public long NetStakeWeight { get; set; }
        public long Weight { get; set; }
        public int ExpectedTime { get; set; }
    }
}
