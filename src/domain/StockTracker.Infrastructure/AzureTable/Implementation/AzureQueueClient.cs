using Azure.Storage.Queues;
using Microsoft.Extensions.Options;
using StockTracker.Models.ApiModels;

namespace StockTracker.Infrastructure.AzureTable.Implementation;

public class AzureQueueClient
{
    private readonly IOptionsMonitor<AzureQueueOptions> _options;

    public QueueClient QueueClient => _queueClientLazy.Value;
    private readonly Lazy<QueueClient> _queueClientLazy;

    internal QueueServiceClient Client => _queueServiceClientLazy.Value;
    private readonly Lazy<QueueServiceClient> _queueServiceClientLazy;
    
    public AzureQueueClient(string queueName, IOptionsMonitor<AzureQueueOptions> options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _queueClientLazy = new Lazy<QueueClient>(() => CreateQueueClient(queueName));
        _queueServiceClientLazy = new Lazy<QueueServiceClient>(CreateQueueServiceClient);
    }

    private QueueServiceClient CreateQueueServiceClient()
    {
        return new QueueServiceClient(_options.CurrentValue.ConnectionString,new QueueClientOptions()
        {
            MessageEncoding = QueueMessageEncoding.Base64
        });
    }

    private QueueClient CreateQueueClient(string queueName)
    {
        var queueClient = Client.GetQueueClient(queueName);
        queueClient.CreateIfNotExists();
        return queueClient;
    }

    public virtual async Task<bool> InsertMessageAsync<T>(T message) where T: class, IRequestContract
    {
        try
        {
            var queueMessage = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(message);

            var operationResult = 
                await QueueClient.SendMessageAsync(new BinaryData(queueMessage));
            return !string.IsNullOrWhiteSpace(operationResult.Value.MessageId);
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}