using System.Threading.Tasks;
using Blockexplorer.BlockProvider.Rpc.Client;
using Blockexplorer.Core.Domain;
using Blockexplorer.Core.Interfaces;
using Microsoft.Extensions.Options;

namespace Blockexplorer.BlockProvider.Rpc
{
    public class StakingInfoAdapter : IStakingInfoAdapter
    {
        readonly BitcoinRpcClient _client;

        public StakingInfoAdapter(IOptions<RpcSettings> rpcSettings)
        {
            _client = new BitcoinRpcClient(rpcSettings.Value);
        }

        public async Task<StakingInfo> GetStakingInfo()
        {
            var rpcStakingInfo = await _client.GetStakingInfo();
            var stakingInfo = new StakingInfo()
            {
                CurrentBlockSize = rpcStakingInfo.CurrentBlockSize,
                                 CurrentBlockTx = rpcStakingInfo.CurrentBlockTx,
                                 PooledTx = rpcStakingInfo.PooledTx,
                                 Difficulty = rpcStakingInfo.Difficulty,
                                 SearchInterval = rpcStakingInfo.SearchInterval,
                                 Weight = rpcStakingInfo.Weight,
                                 NetStakeWeight = rpcStakingInfo.NetStakeWeight,
                                 ExpectedTime = rpcStakingInfo.ExpectedTime
            };

            return stakingInfo;
        }
    }
}
