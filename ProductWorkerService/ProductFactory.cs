using System;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Configuration;
using ProductGrpc.Protos;

namespace ProductWorkerService
{
    public class ProductFactory
    {
        private const float DefaultPrice = 100;

        private readonly IConfiguration _configuration;

        public ProductFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public AddProductRequest Generate()
        {
            var productName = $"{_configuration.GetValue<string>("WorkerService:ProductName")}_{DateTime.UtcNow:yyyyMMddHHmmss}";

            return new AddProductRequest
            {
                Product = new ProductModel
                {
                    Name = productName,
                    Description = $"{productName} added by the worker service",
                    Price = DefaultPrice,
                    Status = ProductStatus.Instock,
                    CreatedTime = Timestamp.FromDateTime(DateTime.UtcNow)
                }
            };
        }
    }
}
