using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.Validators;
using VaiVoa.Domain.Interfaces;
using VaiVoa.Domain.Models;
using VaiVoa.Domain.Models.Validations;

namespace VaiVoa.Domain.Services
{
    public class CreditCardService : BaseService, ICreditCardService
    {
        private readonly ICreditCardRepository _creditCardRepository;
        public CreditCardService(INotificator notificator, 
            ICreditCardRepository creditCardRepository) : base(notificator)
        {
            _creditCardRepository = creditCardRepository;
        }

        public async Task Create(CreditCard creditCard)
        {
            if (!ExecutarValidacao(new CreditCardValidator(), creditCard)) return;

            await _creditCardRepository.Create(creditCard);
        }
        public async Task Update(CreditCard creditCard)
        {
            if (!ExecutarValidacao(new CreditCardValidator(), creditCard)) return;

            await _creditCardRepository.Update(creditCard);
        }

        public async Task<bool> Delete(Guid id)
        {
            await _creditCardRepository.Delete(id);
            return true;
        }

        public void Dispose()
        {
            _creditCardRepository?.Dispose();
        }

        
    }
}
