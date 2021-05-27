using System;
using System.Collections.Generic;
using System.Text;

namespace VaiVoa.Domain.Models
{
    public class CreditCard : Entity
    {
        public CreditCard() { }

        public CreditCard(string number, 
            DateTime validThru, 
            short securityCode, 
            Guid clientId)
        {
            Number = number;
            ValidThru = validThru;
            SecurityCode = securityCode;
            ClientId = clientId;
        }

        public string Number { get; set; }
        public DateTime ValidThru { get; set; }
        public int SecurityCode{ get; set; }

        // Relacionamentos
        public Guid ClientId { get; set; }
        public Client Client { get; set; }
    }
}
