using System.Collections.Concurrent;

using OrderDtos;
using OrderData.Models;
using OrderDtos.DtoExtensions;
using OrderData.Repositories.Contracts;
using OrderServices.Services.Contracts;

using static OrderCommon.ErrorMessageConstants;

namespace OrderServices.Services
{
    public class OrderService : IOrderService
    {
        private readonly ConcurrentDictionary<string, decimal> _latestPrices;

        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;
        private readonly IOrderPublisherService _orderPublisherService;

        public OrderService(
            ConcurrentDictionary<string, decimal> latestPrices,
            IOrderRepository orderRepository,
            IUserRepository userRepository,
            IOrderPublisherService orderPublisherService)
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _latestPrices = latestPrices;
            this._orderPublisherService = orderPublisherService;
        }

        public async Task<OrderDto> AddOrderAsync(int userId, CreateOrderDto createOrderDto)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                user = new User { Id = userId };
                await _userRepository.AddUserAsync(user);
            }

            if (!_latestPrices.TryGetValue(createOrderDto.Ticker, out var price))
            {
                throw new InvalidOperationException(string.Format(NoPricesAvailableForTickerErrorMessage, createOrderDto.Ticker));
            }

            var order = createOrderDto.ToOrder(userId, price);
            await _orderRepository.AddOrderAsync(order);

            var orderExecutedDto = order.ToOrderExecutedDto();
            _orderPublisherService.PublishOrderExecuted(orderExecutedDto);

            return order.ToOrderDto();
        }
    }
}