using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;

namespace cvicenie_mvc.Models
{
    public class Repository
    {
        public static List<AttendanceModel> attendances = new List<AttendanceModel>();
        private readonly string connectionStringApis = "Server=tcp:apissql.database.windows.net,1433;Initial Catalog=apis;Persist Security Info=False;User ID=adminsql;Password=eRYE+7Ax;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        private readonly string connectionStringStats = "Server=tcp:cvicenie1database.database.windows.net,1433;Initial Catalog=apissql;Persist Security Info=False;User ID=db540sj;Password=kyhJ&S4Z;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        private static bool HasColumn(SqlDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (string.Equals(reader.GetName(i), columnName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        public List<AttendanceModel> GetAllAttendance()
        {
            attendances.Clear();
            using (SqlConnection connection = new SqlConnection(connectionStringApis))
            {
                connection.Open();
                string sql = "SELECT * FROM Attendance";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            AttendanceModel attendance = new AttendanceModel
                            {
                                Id = reader.IsDBNull(reader.GetOrdinal("id")) ? 0 : reader.GetInt32(reader.GetOrdinal("id")),
                                Name = reader.IsDBNull(reader.GetOrdinal("name")) ? null : reader.GetString(reader.GetOrdinal("name")),
                                ArrivalTime = reader.IsDBNull(reader.GetOrdinal("arrival_time")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("arrival_time")),
                                DepartureTime = reader.IsDBNull(reader.GetOrdinal("departure_time")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("departure_time"))
                            };
                            attendances.Add(attendance);
                        }
                    }
                }
            }
            return attendances;
        }

        public void ProcessAndCalculateStats()
        {
            // Prečítaj všetky záznamy
            var allRecords = GetAllAttendance();

            // Zoskupuj podľa mena a vypočítaj priemery
            var groupedByName = allRecords
                .GroupBy(r => r.Name)
                .Select(g => new
                {
                    PersonName = g.Key,
                    AverageArrivalTime = CalculateAverageTime(g.Where(r => r.ArrivalTime.HasValue).Select(r => r.ArrivalTime.Value).ToList()),
                    AverageDepartureTime = CalculateAverageTime(g.Where(r => r.DepartureTime.HasValue).Select(r => r.DepartureTime.Value).ToList()),
                    TotalRecords = g.Count()
                })
                .ToList();

            // Zapíš do druhej databázy
            using (SqlConnection connection = new SqlConnection(connectionStringStats))
            {
                connection.Open();

                // Vymaž staré záznamy
                using (SqlCommand deleteCmd = new SqlCommand("DELETE FROM AttendanceStats", connection))
                {
                    deleteCmd.ExecuteNonQuery();
                }

                // Vložíme nové záznamy
                foreach (var record in groupedByName)
                {
                    string sql = @"INSERT INTO AttendanceStats (PersonName, AverageArrivalTime, AverageDepartureTime, TotalRecords, LastUpdated)
                                   VALUES (@PersonName, @AverageArrivalTime, @AverageDepartureTime, @TotalRecords, GETUTCDATE())";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@PersonName", (object?)record.PersonName ?? DBNull.Value);
                        command.Parameters.AddWithValue("@AverageArrivalTime", (object?)record.AverageArrivalTime ?? DBNull.Value);
                        command.Parameters.AddWithValue("@AverageDepartureTime", (object?)record.AverageDepartureTime ?? DBNull.Value);
                        command.Parameters.AddWithValue("@TotalRecords", record.TotalRecords);

                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        private TimeSpan? CalculateAverageTime(List<DateTime> times)
        {
            if (times.Count == 0)
                return null;

            long totalTicks = times.Sum(t => t.TimeOfDay.Ticks);
            long averageTicks = totalTicks / times.Count;
            return new TimeSpan(averageTicks);
        }

        public List<AttendanceStatsModel> GetAttendanceStats()
        {
            var stats = new List<AttendanceStatsModel>();

            using (SqlConnection connection = new SqlConnection(connectionStringStats))
            {
                connection.Open();
                string sql = "SELECT Id, PersonName, AverageArrivalTime, AverageDepartureTime, TotalRecords FROM AttendanceStats";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            stats.Add(new AttendanceStatsModel
                            {
                                Id = reader.GetInt32(0),
                                PersonName = reader.GetString(1),
                                AverageArrivalTime = reader.IsDBNull(2) ? (TimeSpan?)null : reader.GetTimeSpan(2),
                                AverageDepartureTime = reader.IsDBNull(3) ? (TimeSpan?)null : reader.GetTimeSpan(3),
                                TotalRecords = reader.GetInt32(4)
                            });
                        }
                    }
                }
            }

            return stats;
        }
    }
}
