using grpc.server.Entities;
using Microsoft.EntityFrameworkCore;

namespace grpc.server.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<MeterReading> MeterReadings { get; set; }

    }
}
