﻿namespace Blockexplorer.Core.Interfaces
{
    public interface IBlockchainDataProvider : ITransactionProvider, IBlockProvider, IAddressProvider, IStakingInfoAdapter, IInfoAdapter
    {
	  
    }
}
