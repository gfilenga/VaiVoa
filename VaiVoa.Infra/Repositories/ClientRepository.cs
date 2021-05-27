using System;
using System.Collections.Generic;
using System.Text;
using VaiVoa.Domain.Interfaces;
using VaiVoa.Domain.Models;
using VaiVoa.Infra.Context;

namespace VaiVoa.Infra.Repositories
{
    public class ClientRepository : Repository<Client>, IClientRepository
    {
        public ClientRepository(DataContext context) : base(context) { }
    }
}
