using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VaiVoa.Api.Commands
{
    public class CreateCreditCardCommand
    {
        public int SecurityCode { get; set; }
        public Guid ClientId { get; set; }
    }
}
