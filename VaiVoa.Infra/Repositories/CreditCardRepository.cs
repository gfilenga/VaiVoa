using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VaiVoa.Domain.Interfaces;
using VaiVoa.Domain.Models;
using VaiVoa.Infra.Context;

namespace VaiVoa.Infra.Repositories
{
    public class CreditCardRepository : Repository<CreditCard>, ICreditCardRepository
    {
        public CreditCardRepository(DataContext context) : base(context) { }

        public async Task<IEnumerable<CreditCard>> GetAllByEmail(string email)
        {
            return await Context.CreditCards.AsNoTracking().Where(c => c.Client.Email == email).ToListAsync();
        }
    }
}
