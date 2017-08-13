using System.Threading.Tasks;

namespace Blockexplorer.Core.Repositories
{
    public interface IRepository<T>
    {
        Task<T> GetById(string id);
        Task Save(T entity);
    }
}
