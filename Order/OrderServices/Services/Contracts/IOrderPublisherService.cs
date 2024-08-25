using OrderDtos;

namespace OrderServices.Services.Contracts
{
    public interface IOrderPublisherService
    {
        void PublishOrderExecuted(OrderExecutedDto orderExecutedDto);
    }
}