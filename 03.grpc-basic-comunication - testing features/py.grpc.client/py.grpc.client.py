
import grpc

from google.protobuf.timestamp_pb2 import Timestamp
import random

import enum_pb2 as Enum
import MeterReader_pb2 as MeterReader
import MeterReader_pb2_grpc as MeterReaderService

def main():
    print("Calling gRPC Service")

    #need to download .crt from browser, every connection is forced to be secure by protocol,
    with open("localhost.pem", "rb") as file:
        cert = file.read()

    credentials = grpc.ssl_channel_credentials(cert)

    channel = grpc.secure_channel("localhost:5001", credentials)
    stub = MeterReaderService.MeterReadingServiceStub(channel)

    request = MeterReader.ReadingPacket(successful = Enum.ReadingStatus.Success)
    
    now = Timestamp()
    now.GetCurrentTime()
    x = random.randint(1,10000)
    reading = MeterReader.ReadingMessage(customerId = x,
                                        readingValue = 1,
                                        readingTime = now)

    request.readings.append(reading)
    result = stub.AddReading(request)

    if (result.success == Enum.ReadingStatus.Success):
        print("Success")
    else:
        print("Failure")

main()


