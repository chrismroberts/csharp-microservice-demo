using System;

public class OrderInfo
{
    public string OrderId { get; set; }
    public string CustomerName { get; set; }
    public string CustomerEmail { get; set; }
    public string ItemName { get; set; }
    public int QuantityOrdered { get; set; }


    // Queue message specific
    public string QueueMessageId { get; set; }
    public string QueuePopReceipt { get; set; }
}