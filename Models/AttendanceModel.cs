using System;

namespace cvicenie_mvc.Models
{
    public class AttendanceModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime? ArrivalTime { get; set; }
        public DateTime? DepartureTime { get; set; }
    }
}