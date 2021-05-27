using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VaiVoa.Domain.Models;

namespace VaiVoa.Domain.Interfaces
{
    public interface ICreditCardService : IDisposable
    {
        Task Create(CreditCard creditCard);
    }
}
