using System;
using System.Threading.Tasks;
using Coravel.Invocable;
using Microsoft.Extensions.Logging;

public class ProcessOrder : IInvocable
{
    private readonly ILogger<ProcessOrder> logger;
    private readonly IOrderConnector orderConnector;

    public ProcessOrder(ILogger<ProcessOrder> logger, IOrderConnector orderConnector)
    {
        this.logger = logger;
        this.orderConnector = orderConnector;
    }

    public async Task Invoke()
    {
        var nextOrder = await orderConnector.GetNextOrder();

        if (nextOrder != null)
        {
            logger.LogInformation("Processing order {@nextOrder}", nextOrder);

            // TODO: Implement order processing

            await orderConnector.RemoveOrder(nextOrder);
        }
    }
}