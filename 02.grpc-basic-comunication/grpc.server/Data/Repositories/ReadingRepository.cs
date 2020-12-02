using grpc.server.Contracts;
using grpc.server.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace grpc.server.Data.Repositories
{
    public class ReadingRepository : IReadingRepository
    {
        private readonly DataContext _context;

        public ReadingRepository(DataContext context)
        {
            _context = context;
        }

        public void AddEntity(MeterReading model)
        {
            _context.MeterReadings.Add(model);
        }

        public void DeleteEntity(MeterReading model)
        {
            _context.Remove(model);
            //_context.SaveChanges();
        }

        public async Task<bool> SaveAllAsync()
        {
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
