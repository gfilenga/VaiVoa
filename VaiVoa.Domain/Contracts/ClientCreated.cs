using System;
using System.Collections.Generic;
using System.Text;

namespace VaiVoa.Domain.Contracts
{
    public class ClientCreated
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public string Email { get; set; }
    }
}
