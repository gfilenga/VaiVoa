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
    [Route("api/v1/clients")]
    public class ClientController : MainController
    {
        private readonly IClientRepository _clientRepository;
        private readonly IClientService _clientService;

        public ClientController(INotificator notificator,
            IClientRepository clientRepository,
            IClientService clientService) : base(notificator)
        {
            _clientRepository = clientRepository;
            _clientService = clientService;
        }

        [HttpPost]
        public async Task<ActionResult<CreateClientCommand>> Create(CreateClientCommand command)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var client = new Client(command.Name, command.Email, command.Password, command.ConfirmPassword);

            await _clientService.Create(client);

            return CustomResponse(client);
        }

        [HttpGet]
        public async Task<IEnumerable<Client>> List()
        {
            return await _clientRepository.GetAll();
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<UpdateClientCommand>> Update(Guid id,
            UpdateClientCommand command)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            if (id != command.Id) return BadRequest(command);

            var client = await _clientRepository.GetByIdNoTracking(id);

            client.UpdateClient(command.Name, command.Email, command.Password, command.ConfirmPassword);
            
            await _clientService.Update(client);

            return CustomResponse(client);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var client = await _clientRepository.GetByIdNoTracking(id);

            if (client == null) return NotFound();

            await _clientService.Delete(id);

            return CustomResponse();
        }
    }
}
