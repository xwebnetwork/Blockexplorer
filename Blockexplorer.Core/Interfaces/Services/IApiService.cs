using System.Threading.Tasks;
using Blockexplorer.Core.Domain;

namespace Blockexplorer.Core.Interfaces.Services
{
    public interface IApiService
    {
	    Task<Info> GetInfo();
    }
}
