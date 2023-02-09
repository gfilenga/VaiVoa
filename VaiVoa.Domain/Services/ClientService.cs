using Amazon.Runtime.Internal.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaiVoa.Domain.Interfaces;
using VaiVoa.Domain.Mappings;
using VaiVoa.Domain.Messaging;
using VaiVoa.Domain.Models;
using VaiVoa.Domain.Models.Validations;

namespace VaiVoa.Domain.Services
{
    public class ClientService : BaseService, IClientService
    {
        private readonly IClientRepository _clientRepository;
        private readonly ISqsMessenger _sqsMessenger;

        public ClientService(INotificator notificator,
                             IClientRepository clientRepository,
                             ISqsMessenger sqsMessenger) : base(notificator)
        {
            _clientRepository = clientRepository;
            _sqsMessenger = sqsMessenger;
        }

        public async Task Create(Client client)
        {
            if (!ExecutarValidacao(new ClientValidator(), client)) return;

            if (_clientRepository.Search(c => c.Email == client.Email).Result.Any())
            {
                Notificar("Email já cadastrado!");
                return;
            }

            try
            {
                await _clientRepository.Create(client);
                await _sqsMessenger.SendMessageAsync(client.ToCustomerCreatedMessage());
            }
            catch (Exception ex)
            {
                Notificar($"Ocorreu um erro inesperado: {ex.Message}");
                return;
            }
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
