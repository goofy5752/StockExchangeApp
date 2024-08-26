using System.Text.Json;

using Microsoft.Extensions.Caching.Distributed;

using OrderDtos;
using OrderData.Models;
using OrderDtos.DtoExtensions;
using OrderData.Repositories.Contracts;
using OrderServices.Services.Contracts;

using static OrderCommon.GlobalConstants;
using static OrderCommon.ErrorMessageConstants;

namespace OrderServices.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;
        private readonly IOrderPublisherService _orderPublisherService;
        private readonly IDistributedCache _cache;

        public OrderService(
            IOrderRepository orderRepository,
            IUserRepository userRepository,
            IOrderPublisherService orderPublisherService,
            IDistributedCache cache)
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            this._orderPublisherService = orderPublisherService;
            this._cache = cache;
        }

        public async Task<OrderDto> AddOrderAsync(int userId, CreateOrderDto createOrderDto)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                user = new User { Id = userId };
                await _userRepository.AddUserAsync(user);
            }

            var price = await GetLatestPriceFromCacheAsync(createOrderDto.Ticker);
            if (price == null)
            {
                throw new InvalidOperationException(string.Format(NoPricesAvailableForTickerErrorMessage, createOrderDto.Ticker));
            }

            var order = createOrderDto.ToOrder(userId, price.Value);
            await _orderRepository.AddOrderAsync(order);

            var orderExecutedDto = order.ToOrderExecutedDto();
            _orderPublisherService.PublishOrderExecuted(orderExecutedDto);

            return order.ToOrderDto();
        }

        private async Task<decimal?> GetLatestPriceFromCacheAsync(string ticker)
        {
            var cacheKey = $"{CacheKeyPrefix}{ticker}";

            var serializedPrice = await _cache.GetStringAsync(cacheKey);

            if (string.IsNullOrEmpty(serializedPrice))
            {
                return null;
            }

            var stockPrice = JsonSerializer.Deserialize<StockPriceDto>(serializedPrice);
            return stockPrice?.Price;
        }
    }
}