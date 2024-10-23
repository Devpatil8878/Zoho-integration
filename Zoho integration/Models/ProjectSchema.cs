using System.Text.Json.Serialization;

namespace Zoho_integration.Models
{
    public class Portal
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("link")]
        public Link Link { get; set; }
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
        public string Url { get; set; }
    }

    public class Projects
    {
        [JsonPropertyName("projects")]
        public List<Project> ProjectList { get; set; }
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
        public string StartDate { get; set; }

        [JsonPropertyName("end_date")]
        public string EndDate { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("priority")]
        public string? Priority { get; set; }

        [JsonPropertyName("owner_name")]
        public string OwnerName { get; set; }
        
        [JsonPropertyName("owner_id")]
        public string owner_id { get; set; }
        
        [JsonPropertyName("owner_email")]
        public string owner_email { get; set; }
        
        [JsonPropertyName("link")]
        public Link Links { get; set; }

        [JsonPropertyName("tags")]
        public List<Tag> Tags { get; set; }

        [JsonPropertyName("CustomFields")]
        public Dictionary<string, string>? CustomFields { get; set; }
    }

    public class Tag
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("color_class")]
        public string ColorClass { get; set; }
    }

    public class SubTaskContainer
    {
        [JsonPropertyName("tasks")]
        public List<Subtask> Tasks { get; set; }
    }


    public class TaskContainer
    {
        [JsonPropertyName("tasks")]
        public List<Task> Tasks { get; set; }
    }

    public class Status
    {
        [JsonPropertyName("name")]
        public string name { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("color_code")]
        public string ColorCode { get; set; }
    }

    public class Task
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("name")]
        public string title { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("start_date")]
        public string StartDate { get; set; }

        [JsonPropertyName("end_date")]
        public string EndDate { get; set; }

        [JsonPropertyName("work_type")]
        public string WorkType { get; set; }

        [JsonPropertyName("status")]
        public Status St { get; set; }

        public string StatusName => St?.name;

        [JsonPropertyName("priority")]
        public string Priority { get; set; }

        [JsonPropertyName("details")]
        public Details? Assignees { get; set; }

        public Owner? Owner => Assignees.Owners[0];

        public class Details
        {
            [JsonPropertyName("owners")]
            public List<Owner> Owners { get; set; }
        }

        [JsonPropertyName("tags")]
        public List<object> Tags { get; set; }

        [JsonPropertyName("link")]
        public Link Link { get; set; }


        [JsonPropertyName("Reporter")]
        public User? Reporter { get; set; }


        public string Subtasks => Link.SubTask?.Url;

        [JsonPropertyName("Resolution")]
        public string? Resolution { get; set; }

        [JsonPropertyName("StoryPoints")]
        public int? StoryPoints { get; set; }

        [JsonPropertyName("CustomFields")]
        public Dictionary<string, string>? CustomFields { get; set; }

        [JsonPropertyName("log_hours")]
        public LogHours LogHours { get; set; }
    }



    public class Subtask
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("work_type")]
        public string WorkType { get; set; }

        [JsonPropertyName("status")]
        public Status St { get; set; }

        public string StatusName => St?.name;

        [JsonPropertyName("GROUP_NAME")]
        public GroupContainer Assignees { get; set; }

        public class GroupContainer
        {
            public object Teams { get; set; }
        }

        [JsonPropertyName("Reporter")]
        public User Reporter { get; set; }

        [JsonPropertyName("priority")]
        public string Priority { get; set; }

        [JsonPropertyName("tags")]
        public List<Tag> Tags { get; set; }

        [JsonPropertyName("start_date")]
        public string StartDate { get; set; }

        [JsonPropertyName("end_date")]
        public string EndDate { get; set; }

        [JsonPropertyName("Resolution")]
        public string? Resolution { get; set; }

        [JsonPropertyName("StoryPoints")]
        public int? StoryPoints { get; set; }

        [JsonPropertyName("CustomFields")]
        public Dictionary<string, string>? CustomFields { get; set; }

        [JsonPropertyName("log_hours")]
        public LogHours LogHours { get; set; }
    }

    public class Owner
    {

        [JsonPropertyName("name")]
        public string Name { get; set; }


        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }
    }

    public class LogOwner
    {

        [JsonPropertyName("name")]
        public string Name { get; set; }


        [JsonPropertyName("id")]
        public string Id { get; set; }

    }


    public class Timelog
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("log_date_format")]
        public string? DateLogged { get; set; }

        [JsonPropertyName("user")]
        public LogOwner? User { get; set; }

        [JsonPropertyName("hours_display")]
        public string timeSpent { get; set; }
    }

    public class LogHours
    {
        [JsonPropertyName("non_billable_hours")]
        public string NonBillableHours { get; set; }

        [JsonPropertyName("billable_hours")]
        public string BillableHours { get; set; }
    }



    public class Comment
    {
        [JsonPropertyName("CommentText")]
        public string CommentText { get; set; }

        [JsonPropertyName("Author")]
        public User Author { get; set; }

        [JsonPropertyName("Timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("TaskReference")]
        public TaskReference TaskReference { get; set; }
    }

    public class Attachment
    {
        [JsonPropertyName("FileName")]
        public string FileName { get; set; }

        [JsonPropertyName("FileType")]
        public string FileType { get; set; }

        [JsonPropertyName("FileSize")]
        public long FileSize { get; set; }

        [JsonPropertyName("UploadDate")]
        public DateTime UploadDate { get; set; }

        [JsonPropertyName("TaskReference")]
        public TaskReference TaskReference { get; set; }
    }

    public class User
    {
        [JsonPropertyName("id")]
        public string id { get; set; }

        [JsonPropertyName("name")]
        public string name { get; set; }

        [JsonPropertyName("email")]
        public string email { get; set; }

        [JsonPropertyName("portal_role_name")]
        public string role { get; set; }

        
    }

    public class UserContainer
    {
        [JsonPropertyName("users")]
        public List<User> Users { get; set; }
    }

    public class TaskReference
    {
        [JsonPropertyName("TaskId")]
        public string TaskId { get; set; }

        [JsonPropertyName("TaskTitle")]
        public string TaskTitle { get; set; }
    }
    
}
