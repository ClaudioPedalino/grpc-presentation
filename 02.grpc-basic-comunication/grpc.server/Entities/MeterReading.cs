using System;
using System.ComponentModel.DataAnnotations;

namespace grpc.server.Entities
{
    
    public class MeterReading
    {
        [Key]
        public int CustomerId { get; set; }
        public int Value { get; set; }
        public DateTime Time { get; set; }
    }
}
