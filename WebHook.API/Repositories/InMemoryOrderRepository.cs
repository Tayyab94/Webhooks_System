using WebHook.API.Models;

namespace WebHook.API.Repositories
{


    public sealed class InMemoryOrderRepository
    {
        private readonly List<Order> _orders = [];


        public void Add(Order order)
        {
            _orders.Add(order);
        }   

        public IReadOnlyList<Order>GetAll()
        {
            return _orders.AsReadOnly();
        }


        public Task<Order> GetOrderAsync(Guid id)
        {
            var order = _orders.SingleOrDefault(o => o.Id == id);
            return Task.FromResult(order);
        }
        public Task<IEnumerable<Order>> GetOrdersAsync()
        {
            return Task.FromResult(_orders.AsEnumerable());
        }
        public Task<Order> CreateOrderAsync(CreateOrderRequest request)
        {
            var order = new Order(Guid.NewGuid(), request.CustomerName, request.Amount, DateTime.UtcNow);
            _orders.Add(order);
            return Task.FromResult(order);
        }
    }
}
