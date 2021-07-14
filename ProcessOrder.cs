using System;
using System.Threading.Tasks;
using Coravel.Invocable;
using Microsoft.Extensions.Logging;

public class ProcessOrder : IInvocable
{
    private readonly ILogger<ProcessOrder> logger;

    public ProcessOrder(ILogger<ProcessOrder> logger)
    {
        this.logger = logger;
    }

    public Task Invoke()
    {
        return Task.CompletedTask;
    }
}