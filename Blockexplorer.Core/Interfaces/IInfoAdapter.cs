using System.Threading.Tasks;
using Blockexplorer.Core.Domain;

namespace Blockexplorer.Core.Interfaces
{
    public interface IInfoAdapter
    {
	    Task<Info> GetInfo();
    }
}
