using System.Collections.Generic;

namespace Blockexplorer.Core.Domain
{
    public class Address
    {
        public string Id { get; set; }
        public string ColoredAddress { get; set; }
        public decimal Balance { get; set; }
		public int LastModifiedBlockHeight { get; set; }
		public int TotalTransactions { get; set; }
		public string[] TxIds { get; set; }
        public IList<Transaction> Transactions { get; set; }
    }
}
