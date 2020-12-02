using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using grpc.server.Service;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace worker.grpc.client
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _config;
        private readonly ReadingFactory _factory;
        private MeterReadingService.MeterReadingServiceClient _client = null;

        public Worker(ILogger<Worker> logger, IConfiguration config, ReadingFactory factory)
        {
            _logger = logger;
            _config = config;
            _factory = factory;
        }

        protected MeterReadingService.MeterReadingServiceClient Client
        {
            get
            {
                if (_client == null)
                {
                    var channel = GrpcChannel.ForAddress(_config.GetValue<string>("Service:ServiceUrl"));
                    _client = new MeterReadingService.MeterReadingServiceClient(channel);
                }
                return _client;
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                var pkt = new ReadingPacket()
                {
                    Successful = ReadingStatus.Success,
                    Notes = "This is a nice test"
                };

                for (int i = 0; i < _config.GetValue<int>("Service:Batch"); i++)
                {
                    pkt.Readings.Add(await _factory.Generate(new Random().Next(1,10000)));
                }
                
                var result = await Client.AddReadingAsync(pkt);

                if (result.Success == ReadingStatus.Success)
                {
                    _logger.LogInformation("Succesfully sent");
                }
                else
                {
                    _logger.LogInformation("Failed to send");
                }

                await Task.Delay(_config.GetValue<int>("Service:DelayInterval"), stoppingToken);
            }
        }
    }
}
