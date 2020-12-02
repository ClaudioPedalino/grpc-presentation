using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using grpc.server.Service;
using Grpc.Core;
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
        private readonly ILoggerFactory _loggerFactory;
        private MeterReadingService.MeterReadingServiceClient _client = null;

        public Worker(ILogger<Worker> logger,
                      IConfiguration config,
                      ReadingFactory factory,
                      ILoggerFactory loggerFactory)
        {
            _logger = logger;
            _config = config;
            _factory = factory;
            _loggerFactory = loggerFactory;
        }

        protected MeterReadingService.MeterReadingServiceClient Client
        {
            get
            {
                if (_client == null)
                {
                    var opt = new GrpcChannelOptions()
                    {
                        LoggerFactory = _loggerFactory
                    };
                    
                    //var channel = GrpcChannel.ForAddress(_config.GetValue<string>("Service:ServiceUrl")); => Basic
                    var channel = GrpcChannel.ForAddress(_config.GetValue<string>("Service:ServiceUrl"), opt); //=> Custom w/looger

                    _client = new MeterReadingService.MeterReadingServiceClient(channel);
                }
                return _client;
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var counter = 0;
            var customerId = new Random();

            while (!stoppingToken.IsCancellationRequested)
            {
                counter++;

                if (counter % 10 == 0)
                {
                    Console.WriteLine("Sending Diagnostics..");
                    var stream = Client.SendDiagnostics();
                    for (int i = 0; i < 5; i++)
                    {
                        var reading = await _factory.Generate(customerId.Next(1, 10000));
                        await stream.RequestStream.WriteAsync(reading);
                    }

                    await stream.RequestStream.CompleteAsync();
                }

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                var pkt = new ReadingPacket()
                {
                    Successful = ReadingStatus.Success,
                    Notes = "This is a nice test"
                };

                for (int i = 0; i < _config.GetValue<int>("Service:Batch"); i++)
                {
                    pkt.Readings.Add(await _factory.Generate(customerId.Next(1, 10000)));
                }

                try
                {
                    var result = await Client.AddReadingAsync(pkt);

                    if (result.Success == ReadingStatus.Success)
                    {
                        _logger.LogInformation("Succesfully sent");
                    }
                    else
                    {
                        _logger.LogInformation("Failed to send");
                    }
                }
                catch (RpcException ex)
                {
                    //_logger.LogError($"Exception thrown: {ex}"); // basic status error

                    if (ex.StatusCode == StatusCode.OutOfRange)
                    {
                        _logger.LogError($"{ex.Trailers}");
                    }
                    _logger.LogError($"Exception thrown: {ex}"); // custom status error

                }

                await Task.Delay(_config.GetValue<int>("Service:DelayInterval"), stoppingToken);
            }
        }
    }
}
