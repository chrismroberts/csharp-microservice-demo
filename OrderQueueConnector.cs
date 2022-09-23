using System;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Microsoft.Extensions.Logging;

public class OrderQueueConnector : IOrderConnector
{
    private readonly ILogger<OrderQueueConnector> logger;
    private readonly QueueClient orderQueueClient;
    private readonly QueueClient poisonOrderQueueClient;

    public OrderQueueConnector(ILogger<OrderQueueConnector> logger)
    {
        this.logger = logger;

        var connectionString = Environment.GetEnvironmentVariable("STORAGE_CONNECTION");
        var queueOpts = new QueueClientOptions { MessageEncoding = QueueMessageEncoding.Base64 };

        orderQueueClient = new QueueClient(connectionString, "customer-orders", queueOpts);
        orderQueueClient.CreateIfNotExists();

        poisonOrderQueueClient = new QueueClient(connectionString, "customer-orders-poison", queueOpts);
        poisonOrderQueueClient.CreateIfNotExists();
    }
    
    public async Task<OrderInfo> GetNextOrder()
    {
        var response = await orderQueueClient.ReceiveMessageAsync();

        if (response.Value != null)
        {
            try
            {                
                var order = response.Value.Body.ToObjectFromJson<OrderInfo>();
                order.QueueMessageId = response.Value.MessageId;
                order.QueuePopReceipt = response.Value.PopReceipt;

                return order;
            }
            catch (System.Exception ex)
            {
                logger.LogError(ex, "Error deserializing message", response.Value);

                await poisonOrderQueueClient.SendMessageAsync(response.Value.Body);
                await orderQueueClient.DeleteMessageAsync(response.Value.MessageId, response.Value.PopReceipt);
            }
        }

        return null;
    }

    public async Task RemoveOrder(OrderInfo order)
    {
        await orderQueueClient.DeleteMessageAsync(order.QueueMessageId, order.QueuePopReceipt);
    }
}