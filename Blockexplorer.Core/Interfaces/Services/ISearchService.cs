using System.Threading.Tasks;
using Blockexplorer.Core.Enums;

namespace Blockexplorer.Core.Interfaces.Services
{
    public interface ISearchService
    {
        Task<EntitySearchResult> SearchEntityById(string id);
    }
}