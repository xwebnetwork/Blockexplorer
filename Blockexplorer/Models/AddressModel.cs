using Blockexplorer.Core.Domain;

namespace Blockexplorer.Models
{
    public class AddressModel
    {
        public Address Address { get; set; }
        public int Count { get; set; }
        public int Start { get; set; }
        public int CurrentPage { get; set; }
        public long Max { get; set; }
    }
}