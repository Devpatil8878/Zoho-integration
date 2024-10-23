using System.Text.Json.Serialization;

namespace Zoho_integration.Models
{
    public class GProject
    {

        public long Id { get; set; }


        public string Name { get; set; }

        public string? Description { get; set; }

        public string StartDate { get; set; }


        public string EndDate { get; set; }


        public string Status { get; set; }


        public string? Priority { get; set; }

        public GOwner Owner { get; set; }

        public List<GTask> Tasks { get; set; }

        public List<Tag> Tags { get; set; }

        public Dictionary<string, string>? CustomFields { get; set; }

        public List<User> resources { get; set; }
    }

    public class GOwner
    {

        public string id { get; set; }
        public string name { get; set; }
        public string? email { get; set; }

    }



    public class GTask
    {

        public string id { get; set; }

        public string title { get; set; }

        public string description { get; set; }

        public List<Owner> assignees { get; set; }

        public List<Owner> reporter { get; set; }
        public string status { get; set; }

        public string WorkType { get; set; }



        public string priority { get; set; }


        public class Details
        {
            public List<Owner> Owners { get; set; }
        }
        public List<string> Tags { get; set; }
        public string startDate { get; set; }

        public string dueDate { get; set; }

        public List<GSubtask>? Subtasks { get; set; }

        public string? resolution { get; set; }


        public int? StoryPoints { get; set; }

        public Dictionary<string, string>? CustomFields { get; set; }

        public List<GTimeLog> timelogs { get; set; }
    }

    public class GSubtask { 
        public string id { get; set; }
        public string title { get; set; }
        public string status { get; set; }

        public List<GAssignee> assignees { get; set; }
    }

    public class GAssignee
    {
        public string id { get; set; }
        public string name { get; set; }

    }

    public class GTimeLog
    {
        public string id { get; set; }

        public string? DateLogged { get; set; }

        public LogOwner? user { get; set; }

        public string timeSpent { get; set; }
    }


}
