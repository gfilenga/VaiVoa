using System;
using System.Collections.Generic;
using System.Text;
using VaiVoa.Domain.Interfaces;
using VaiVoa.Domain.Models;
using VaiVoa.Infra.Context;

namespace VaiVoa.Infra.Repositories
{
    public class CreditCardRepository : Repository<CreditCard>, ICreditCardRepository
    {
        public CreditCardRepository(DataContext context) : base(context) { }
    }
}
