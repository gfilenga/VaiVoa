using System;
using System.Collections.Generic;
using System.Text;
using VaiVoa.Domain.Contracts;
using VaiVoa.Domain.Models;

namespace VaiVoa.Domain.Mappings
{
    public static class DomainToMessageMapper
    {
        public static ClientCreated ToCustomerCreatedMessage(this Client client)
        {
            return new ClientCreated
            {
                Id = client.Id,
                Email = client.Email,
                Name = client.Name
            };
        }
    }
}
