using System.Collections.Concurrent;

using Microsoft.EntityFrameworkCore;

using OrderData;
using OrderData.Repositories;
using OrderData.Repositories.Contracts;
using OrderServices.Services;
using OrderServices.Services.Contracts;

namespace OrderApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var latestPrices = new ConcurrentDictionary<string, decimal>();
            builder.Services.AddSingleton(latestPrices);

            builder.Services.AddControllers();

            builder.Services.AddDbContext<OrderDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IOrderPublisherService, OrderPublisherService>();

            builder.Services.AddHostedService<PriceConsumerService>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}