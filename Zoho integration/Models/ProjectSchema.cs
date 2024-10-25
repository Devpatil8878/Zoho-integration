using System.Text.Json.Serialization;

namespace Zoho_integration.Models
{
    public class Portal
    {
        [JsonPropertyName("id")]
        public long? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("link")]
        public Link? Link { get; set; }
    }

    public class Link
    {
        [JsonPropertyName("project")]
        public ProjectUrl? Project { get; set; }

        [JsonPropertyName("activity")]
        public ProjectUrl? Activity { get; set; }

        [JsonPropertyName("document")]
        public ProjectUrl? Document { get; set; }

        [JsonPropertyName("forum")]
        public ProjectUrl? Forum { get; set; }

        [JsonPropertyName("timesheet")]
        public ProjectUrl? Timesheet { get; set; }

        [JsonPropertyName("task")]
        public ProjectUrl? Task { get; set; }

        [JsonPropertyName("folder")]
        public ProjectUrl? Folder { get; set; }

        [JsonPropertyName("milestone")]
        public ProjectUrl? Milestone { get; set; }

        [JsonPropertyName("bug")]
        public ProjectUrl? Bug { get; set; }

        [JsonPropertyName("self")]
        public ProjectUrl? Self { get; set; }

        [JsonPropertyName("tasklist")]
        public ProjectUrl? Tasklist { get; set; }

        [JsonPropertyName("event")]
        public ProjectUrl? Event { get; set; }

        [JsonPropertyName("user")]
        public ProjectUrl? User { get; set; }

        [JsonPropertyName("status")]
        public ProjectUrl? Status { get; set; }
        
        [JsonPropertyName("subtask")]
        public ProjectUrl? SubTask { get; set; }
    }

    public class ProjectUrl
    {
        [JsonPropertyName("url")]
        public string? Url { get; set; }
    }

    public class Projects
    {
        [JsonPropertyName("projects")]
        public List<Project>? ProjectList { get; set; }
    }

    public class Project
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("start_date")]
        public string? StartDate { get; set; }

        [JsonPropertyName("end_date")]
        public string? EndDate { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("priority")]
        public string? Priority { get; set; }

        [JsonPropertyName("owner_name")]
        public string? OwnerName { get; set; }
        
        [JsonPropertyName("owner_id")]
        public string? owner_id { get; set; }
        
        [JsonPropertyName("owner_email")]
        public string? owner_email { get; set; }
        
        [JsonPropertyName("link")]
        public Link? Links { get; set; }

        [JsonPropertyName("TAGS")]
        public List<Tag>? Tags { get; set; }

        [JsonPropertyName("custom_fields")]
        public List<CustomField>? CustomFields { get; set; }
    }

    public class Tag
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("color_class")]
        public string? ColorClass { get; set; }
    }

    public class Status
    {
        [JsonPropertyName("name")]
        public string? name { get; set; }

        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("color_code")]
        public string? ColorCode { get; set; }
    }

    public class CustomField
    {
        [JsonPropertyName("label_name")]
        public string? fieldName { get; set; }
        
        [JsonPropertyName("value")]
        public string? value { get; set; }

    }

    public class Owner
    {

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }
    }

    public class User
    {
        [JsonPropertyName("id")]
        public string? id { get; set; }

        [JsonPropertyName("name")]
        public string? name { get; set; }

        [JsonPropertyName("email")]
        public string? email { get; set; }

        [JsonPropertyName("portal_role_name")]
        public string? role { get; set; }
    }

    public class UserContainer
    {
        [JsonPropertyName("users")]
        public List<User>? Users { get; set; }
    }

    public class Attachment
    {
        [JsonPropertyName("id")]
        public string? id { get; set; }

        [JsonPropertyName("FileName")]
        public string? fileName { get; set; }

        [JsonPropertyName("FileType")]
        public string? fileType { get; set; }

        [JsonPropertyName("FileSize")]
        public long? fileSize { get; set; }

        [JsonPropertyName("UploadDate")]
        public DateTime? uploadDate { get; set; }

        [JsonPropertyName("TaskReference")]
        public TaskReference? url { get; set; }
    }


}
