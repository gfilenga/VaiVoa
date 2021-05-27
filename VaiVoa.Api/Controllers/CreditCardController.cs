using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VaiVoa.Api.Commands;
using VaiVoa.Domain.Interfaces;
using VaiVoa.Domain.Models;

namespace VaiVoa.Api.Controllers
{
    [Route("api/v1/credit-card")]
    public class CreditCardController : MainController
    {
        private readonly ICreditCardRepository _creditCardRepository;
        private readonly ICreditCardService _creditCardService;
        private readonly IClientRepository _clientRepository;

        public CreditCardController(INotificator notificator,
            ICreditCardRepository creditCardRepository,
            ICreditCardService creditCardService, 
            IClientRepository clientRepository) : base(notificator)
        {
            _creditCardRepository = creditCardRepository;
            _creditCardService = creditCardService;
            _clientRepository = clientRepository;
        }
        [HttpPost]
        public async Task<ActionResult<CreateCreditCardCommand>> Create(string email,
            CreateCreditCardCommand command)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);
            
            if (!_clientRepository.Search(c => c.Email == email).Result.Any()) return NotFound();
           
            var creditCard = new CreditCard(command.SecurityCode, command.ClientId);

            await _creditCardService.Create(creditCard);

            return CustomResponse(creditCard);
        }

        [HttpGet]
        public async Task<IEnumerable<CreditCard>> ListAllByEmail(string email)
        {
            return await _creditCardRepository.GetAllByEmail(email);
        }
    }
}
