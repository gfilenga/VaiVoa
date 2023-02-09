using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VaiVoa.Domain.Messaging;

namespace VaiVoa.Infra.Messaging
{
    public class SqsMessenger : ISqsMessenger
    {
        private readonly IAmazonSQS _sqs;
        private readonly IOptions<QueueSettings> _queueSettings;
        private string _queueUrl;

        public SqsMessenger(IAmazonSQS sqs, IOptions<QueueSettings> queueSettings)
        {
            _sqs = sqs;
            _queueSettings = queueSettings;
        }

        public async Task<SendMessageResponse> SendMessageAsync<T>(T message)
        {
            var queueUrl = await GetQueueUrlAsync();

            var sendMessageRequest = new SendMessageRequest
            {
                QueueUrl = queueUrl,
                MessageBody = JsonSerializer.Serialize(message),
                MessageAttributes = new Dictionary<string, MessageAttributeValue>
                {
                    {
                        "MessageType", new MessageAttributeValue
                        {
                            DataType = "String",
                            StringValue = typeof(T).Name
                        }
                    }
                }
            };

            return await _sqs.SendMessageAsync(sendMessageRequest);
        }

        private async Task<string> GetQueueUrlAsync()
        {
            if (!String.IsNullOrEmpty(_queueUrl))
            {
                return _queueUrl;
            }

            var queueUrlResponse = await _sqs.GetQueueUrlAsync(_queueSettings.Value.Name);
            _queueUrl = queueUrlResponse.QueueUrl;
            return _queueUrl;
        }
    }
}
