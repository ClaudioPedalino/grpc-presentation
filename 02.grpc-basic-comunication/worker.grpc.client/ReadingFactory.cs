using Google.Protobuf.WellKnownTypes;
using grpc.server.Service;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace worker.grpc.client
{
    public class ReadingFactory
    {
        private readonly ILogger<ReadingFactory> logger;

        public ReadingFactory(ILogger<ReadingFactory> _logger)
        {
            logger = _logger;
        }

        public Task<ReadingMessage> Generate(int customerId)
        {
            var reading = new ReadingMessage()
            {
                CustomerId = customerId,
                ReadingValue = new Random().Next(1, 1000),
                ReadingTime = Timestamp.FromDateTime(DateTime.UtcNow)
            };

            return Task.FromResult(reading);
        }
    }
}
