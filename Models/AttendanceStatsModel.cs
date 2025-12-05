using System;

namespace cvicenie_mvc.Models
{
    public class AttendanceStatsModel
    {
        public int Id { get; set; }
        public string? PersonName { get; set; }
        public TimeSpan? AverageArrivalTime { get; set; }
        public TimeSpan? AverageDepartureTime { get; set; }
        public int TotalRecords { get; set; }
    }
}