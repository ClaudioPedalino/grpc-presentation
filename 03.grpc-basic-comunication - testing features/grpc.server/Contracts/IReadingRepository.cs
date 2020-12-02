using grpc.server.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace grpc.server.Contracts
{
    public interface IReadingRepository
    {

        void AddEntity(MeterReading model);
        void DeleteEntity(MeterReading model);
        Task<bool> SaveAllAsync();
    }
}
