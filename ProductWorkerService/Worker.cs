using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using ProductGrpc.Protos;

namespace ProductWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private readonly ProductFactory _factory;

        public Worker(ILogger<Worker> logger, IConfiguration configuration, ProductFactory factory)
        {
            _logger = logger;
            _configuration = configuration;
            _factory = factory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var channel = GrpcChannel.ForAddress(_configuration.GetValue<string>("WorkerService:ServerUrl"));
            var client = new ProductProtoService.ProductProtoServiceClient(channel);
            var interval = _configuration.GetValue<int>("WorkerService:TaskInterval");

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                try
                {
                    _logger.LogInformation("AddProductAsync started..");
                    var addProductResponse = await client.AddProductAsync(_factory.Generate(), cancellationToken: stoppingToken);
                    _logger.LogInformation("AddProduct Response: {product}", addProductResponse.ToString());
                }
                catch (RpcException exception) when (!stoppingToken.IsCancellationRequested)
                {
                    // Keep running: the server may not be up yet, the next tick tries again.
                    _logger.LogError(exception, "AddProduct failed, retrying in {interval} ms", interval);
                }

                await Task.Delay(interval, stoppingToken);
            }
        }
    }
}
