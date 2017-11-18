using System;
using System.Collections.Generic;
using System.Text;

namespace Blockexplorer.Core.Domain
{
    public enum TransactionType
    {
		Unknown = 0,
		PoW_Reward_Coinbase = 1,
		PoS_Reward = 2,
		Money = 3
    }
}
