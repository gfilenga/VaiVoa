using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VaiVoa.Domain.Models;

namespace VaiVoa.Domain.Interfaces
{
    public interface IClientService : IDisposable
    {
        Task Create(Client client);
        Task Update(Client client);
        Task<bool> Delete(Guid id);
    }
}
