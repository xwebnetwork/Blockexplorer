using Blockexplorer.Core.Domain;

namespace Blockexplorer.Models
{
    public class BlockModel
    {
        public Block Block { get; set; }
        public int Count { get; set; }
        public int Start { get; set; }
        public int CurrentPage { get; set; }
        public int Max { get; set; }
    }
}