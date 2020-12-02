using grpc.server.Contracts;
using grpc.server.Entities;
using grpc.server.Service;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace grpc.server.Services
{
    public class MeterService : MeterReadingService.MeterReadingServiceBase
    {
        private readonly ILogger<MeterService> _logger;
        private readonly IReadingRepository _repository;

        public MeterService(ILogger<MeterService> logger, IReadingRepository repository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async override Task<StatusMessage> AddReading(ReadingPacket request, ServerCallContext context)
        {
            var result = new StatusMessage()
            {
                Success = ReadingStatus.Failure
            };

            if (request.Successful == ReadingStatus.Success)
            {
                try
                {
                    foreach (var r in request.Readings)
                    {
                        var reading = new MeterReading()
                        {
                            CustomerId = r.CustomerId,
                            Value = r.ReadingValue,
                            Time = r.ReadingTime.ToDateTime()
                        };
                        _repository.AddEntity(reading);
                    }

                    if (await _repository.SaveAllAsync())
                    {
                        _logger.LogInformation($"Stored {request.Readings.Count} new readings...");
                        result.Success = ReadingStatus.Success;
                    }

                }
                catch (Exception ex)
                {
                    result.Message = "Exception thrown during process";
                    _logger.LogError($"Expecion thrown during of reading {ex}");
                }
            }


            return await Task.FromResult(result);
        }


    }
}
