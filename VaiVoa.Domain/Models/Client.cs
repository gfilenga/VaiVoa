using System;
using System.Collections.Generic;
using System.Text;

namespace VaiVoa.Domain.Models
{
    public class Client : Entity
    {
        public Client() { }

        public Client(string name, 
            string email, 
            string password, 
            string confirmPassword)
        {
            Name = name;
            Email = email;
            Password = password;
            ConfirmPassword = confirmPassword;
            CreditCards = new List<CreditCard>();
        }

        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        // Relacionamentos
        public IList<CreditCard> CreditCards { get; set; }

        public void UpdateClient(string name,
            string email,
            string password,
            string confirmPassword)
        {
            Name = name;
            Email = email;
            Password = password;
            ConfirmPassword = confirmPassword;
        }
    }
}
