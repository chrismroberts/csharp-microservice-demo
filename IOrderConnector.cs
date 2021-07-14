using System.Threading.Tasks;

public interface IOrderConnector
{
    Task<OrderInfo> GetNextOrder();
    Task RemoveOrder(OrderInfo order);
}