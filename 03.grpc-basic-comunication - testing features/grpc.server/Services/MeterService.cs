using Google.Protobuf.WellKnownTypes;
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

        public override Task<Empty> Test(Empty request, ServerCallContext context)
        {
            return base.Test(request, context);
        }

        public override async Task<Empty> SendDiagnostics(IAsyncStreamReader<ReadingMessage> requestStream, ServerCallContext context)
        {
            var theTask = Task.Run(async () =>
            {
                await foreach (var reading in requestStream.ReadAllAsync())
                {
                    _logger.LogInformation($"Recived Reading:{reading}");
                }
            });

            await theTask;

            return new Empty();
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

                        //---------test
                        if (r.ReadingValue < 1000)
                        {
                            _logger.LogDebug("Reading Value below acceptable level");
                            //throw new RpcException(Status.DefaultCancelled, "Value too low.."); => basic status error

                            #region custom status error
                            var trailer = new Metadata()
                            {
                                {"BadValue", r.ReadingValue.ToString()},
                                {"Field", "ReadingValue"},
                                {"Message", "Readings are invalid"}
                            };
                            throw new RpcException(new Status(StatusCode.OutOfRange, "Value too low")); // => custom status error
                            #endregion
                        }
                        //---------test

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
                //test
                catch (RpcException)
                {
                    throw;
                }
                //
                catch (Exception ex)
                {
                    _logger.LogError($"Expecion thrown during of reading {ex}");
                    throw new RpcException(Status.DefaultCancelled, "Exception thrown during process");
                }
            }


            return await Task.FromResult(result);
        }


    }
}
