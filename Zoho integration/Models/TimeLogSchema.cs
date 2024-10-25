using System.Text.Json.Serialization;

namespace Zoho_integration.Models
{ 
    public class GTimeLog
    {
        public string id { get; set; }
        public string? DateLogged { get; set; }
        public LogOwner? user { get; set; }
        public string? timeSpent { get; set; }
    }
    public class LogOwner
    {

        [JsonPropertyName("name")]
        public string? Name { get; set; }


        [JsonPropertyName("zuid")]
        public long? Id { get; set; }

    }
    public class TimeLogContainer
    {
        [JsonPropertyName("timelogs")]
        public TimeLogs? Timelogs { get; set; }
    }
    public class TimeLogs
    {
        [JsonPropertyName("total_log_hours")]
        public string? TotalLogHours { get; set; }
        [JsonPropertyName("tasklogs")]
        public List<TaskLog>? TaskLogs { get; set; }
    }
    public class TaskLog
    {
        [JsonPropertyName("id")]
        public long? Id { get; set; }

        [JsonPropertyName("log_date_format")]
        public string? DateLogged { get; set; }

        [JsonPropertyName("added_by")]
        public LogOwner? User { get; set; }

        [JsonPropertyName("hours_display")]
        public string? timeSpent { get; set; }
    }
    public class LogHours
    {
        [JsonPropertyName("non_billable_hours")]
        public string? NonBillableHours { get; set; }

        [JsonPropertyName("billable_hours")]
        public string? BillableHours { get; set; }
    }
}
