using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VaiVoa.Domain.Interfaces;

namespace VaiVoa.Api.Controllers
{
    [Route("api/v1/credit-cards")]
    public class CreditCardController : MainController
    {
        private readonly ICreditCardRepository _creditCardRepository;
        private readonly ICreditCardService _creditCardService;
        public CreditCardController(INotificator notificator, 
            ICreditCardRepository creditCardRepository, 
            ICreditCardService creditCardService) : base(notificator)
        {
            _creditCardRepository = creditCardRepository;
            _creditCardService = creditCardService;
        }
    }
}
