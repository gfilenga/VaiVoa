using System;
using System.Collections.Generic;
using System.Text;

namespace VaiVoa.Domain.Models
{
    public class CreditCard : Entity
    {
        public CreditCard() { }

        public CreditCard(int securityCode, 
            Guid clientId)
        {
            Number = GenerateCreditCardNumber();
            ValidThru = DateTime.Now.AddDays(2555);
            SecurityCode = securityCode;
            ClientId = clientId;
        }

        public string Number { get; set; }
        public DateTime ValidThru { get; set; }
        public int SecurityCode{ get; set; }

        // Relacionamentos
        public Guid ClientId { get; set; }
        public Client Client { get; set; }

        private string GenerateCreditCardNumber()
        {
            string creditCardNumber = "4";
            for (int i = 0; i < 15; i++)
            {
                Random rnd = new Random();
                var number = rnd.Next(1, 10);
                creditCardNumber += number.ToString();
            }

            return creditCardNumber;
        }
    }
}
