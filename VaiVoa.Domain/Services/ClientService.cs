using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaiVoa.Domain.Interfaces;
using VaiVoa.Domain.Models;
using VaiVoa.Domain.Models.Validations;

namespace VaiVoa.Domain.Services
{
    public class ClientService : BaseService, IClientService
    {
        private readonly IClientRepository _clientRepository;
        public ClientService(INotificator notificator, 
            IClientRepository clientRepository) : base(notificator)
        {
            _clientRepository = clientRepository;
        }

        public async Task Create(Client client)
        {
            if (!ExecutarValidacao(new ClientValidator(), client)) return;

            if (_clientRepository.Search(c => c.Email == client.Email).Result.Any())
            {
                Notificar("Email já cadastrado!");
                return;
            }

            await _clientRepository.Create(client);
        }
             
        public async Task Update(Client client)
        {
            if (!ExecutarValidacao(new ClientValidator(), client)) return;

            await _clientRepository.Update(client);
        }

        public async Task<bool> Delete(Guid id)
        {
            await _clientRepository.Delete(id);
            return true;
        }
        public void Dispose()
        {
            _clientRepository?.Dispose();
        }
    }
}
