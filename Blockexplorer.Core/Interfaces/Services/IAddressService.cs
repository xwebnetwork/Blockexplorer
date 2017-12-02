using System.Threading.Tasks;
using Blockexplorer.Core.Domain;
using System.Collections.Generic;
using System;

namespace Blockexplorer.Core.Interfaces.Services
{
    public interface IAddressService
    {
        Task<Address> GetAddress(string id);
        Task<List<Address>> GetTopList();
        Task<Tuple<int, DateTime>> GetStats();
    }
}
