using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace SendMessage.Data
{
    public class ServiceBusService
    {
        readonly ServiceBusClient _client;
        const string queueName = "storage-trigger";

        public ServiceBusService(IConfiguration config)
        {
            string connectionString = config.GetConnectionString("Queue_Connection_String");
            _client = new ServiceBusClient(connectionString);
        }

        public async Task SendMessageAsync(BlobFile file)
        {
            // The sender is responsible for publishing messages to the queue.
            ServiceBusSender sender = _client.CreateSender(queueName);
            ServiceBusMessage message = new ServiceBusMessage(file.Name + " from SendMessage");
            await sender.SendMessageAsync(message);
        }
    }
}
