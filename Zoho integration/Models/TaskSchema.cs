using System.Text.Json.Serialization;

namespace Zoho_integration.Models
{
    public class Task
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("name")]
        public string title { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("start_date")]
        public string? StartDate { get; set; }

        [JsonPropertyName("end_date")]
        public string? EndDate { get; set; }

        [JsonPropertyName("work_type")]
        public string? WorkType { get; set; }

        [JsonPropertyName("status")]
        public Status? St { get; set; }

        public string? StatusName => St?.name;

        [JsonPropertyName("priority")]
        public string? Priority { get; set; }

        [JsonPropertyName("USER")]
        public User? Assignees { get; set; }

        public class Details
        {
            [JsonPropertyName("owners")]
            public List<Owner>? Owners { get; set; }
        }

        [JsonPropertyName("tags")]
        public List<Tag>? Tags { get; set; }

        [JsonPropertyName("custom_fields")]
        public List<CustomField>? CustomFields { get; set; }

        [JsonPropertyName("link")]
        public Link? Link { get; set; }

        [JsonPropertyName("details")]
        public Details? Dets { get; set; }
        public Owner? Reporter => Dets.Owners[0];
        public string? Subtasks => Link.SubTask?.Url;

        [JsonPropertyName("Resolution")]
        public string? Resolution { get; set; }

        [JsonPropertyName("StoryPoints")]
        public int? StoryPoints { get; set; }

        [JsonPropertyName("log_hours")]
        public LogHours? LogHours { get; set; }
    }
    public class TaskContainer
    {
        [JsonPropertyName("tasks")]
        public List<Task>? Tasks { get; set; }
    }
    public class TaskReference
    {
        [JsonPropertyName("TaskId")]
        public string? TaskId { get; set; }

        [JsonPropertyName("TaskTitle")]
        public string? TaskTitle { get; set; }
    }
    public class GTask
    {
        public string id { get; set; }
        public string title { get; set; }
        public string? description { get; set; }
        public string? type { get; set; }
        public string? status { get; set; }
        public string? assignees { get; set; }
        public Owner? reporter { get; set; }
        public string? priority { get; set; }
        public class Details
        {
            public List<Owner>? Owners { get; set; }
        }
        public List<string>? tags { get; set; }
        public string? startDate { get; set; }
        public string? dueDate { get; set; }
        public string? timeEstimate { get; set; }
        public string? timeSpent { get; set; }
        public string? resolution { get; set; }
        public List<GSubtask>? subtasks { get; set; }
        public List<GComment>? comments { get; set; }
        public List<Attachment>? attactments { get; set; }
        public List<GTimeLog>? timelogs { get; set; }
        public Dictionary<string, string>? customFields { get; set; }
    }
}
