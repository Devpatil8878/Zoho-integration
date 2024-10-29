using System.Text.Json.Serialization;

namespace Zoho_integration.Models
{
    public class GSubtask
    {
        public string id { get; set; }
        public string title { get; set; }
        public string? status { get; set; }
        public string? assignees { get; set; }
    }
    public class SubTaskContainer
    {
        [JsonPropertyName("tasks")]
        public List<Subtask>? Tasks { get; set; }
    }
    public class Subtask
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("work_type")]
        public string? WorkType { get; set; }

        [JsonPropertyName("status")]
        public Status? St { get; set; }
        public string? StatusName => St?.name;

        [JsonPropertyName("GROUP_NAME")]
        public GroupContainer? Assignees { get; set; }

        public class GroupContainer
        {
            public object? Teams { get; set; }
        }

        [JsonPropertyName("Reporter")]
        public User? Reporter { get; set; }

        [JsonPropertyName("priority")]
        public string? Priority { get; set; }

        [JsonPropertyName("tags")]
        public List<Tag>? Tags { get; set; }

        [JsonPropertyName("start_date")]
        public string? StartDate { get; set; }

        [JsonPropertyName("end_date")]
        public string? EndDate { get; set; }

        [JsonPropertyName("Resolution")]
        public string? Resolution { get; set; }

        [JsonPropertyName("StoryPoints")]
        public int? StoryPoints { get; set; }

        [JsonPropertyName("CustomFields")]
        public Dictionary<string, string>? CustomFields { get; set; }

        [JsonPropertyName("log_hours")]
        public LogHours? LogHours { get; set; }
    }
}
