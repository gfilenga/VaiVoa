using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VaiVoa.Domain.Models;

namespace VaiVoa.Domain.Interfaces
{
    public interface ICreditCardRepository : IRepository<CreditCard>
    {
        Task<IEnumerable<CreditCard>> GetAllByEmail(string email);
    }
}
