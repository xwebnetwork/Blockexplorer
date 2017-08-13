using System.Collections.Generic;

namespace Blockexplorer.Core.Domain
{
    public class Address
    {
        public string Hash { get; set; }
        public string ColoredAddress { get; set; }
        public string UncoloredAddress { get; set; }
        public long Balance { get; set; }
        public long TotalTransactions { get; set; }
        public IList<Transaction> Transactions { get; set; }
    }
}
