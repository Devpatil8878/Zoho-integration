using System.Text.Json.Serialization;

namespace Zoho_integration.Models
{
    public class GProjectSchema
    {
        public GProject? project { get; set; }
    }

    public class GProject
    {
        public String Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public string? Status { get; set; }
        public string? Priority { get; set; }
        public GOwner? Owner { get; set; }
        public List<string>? Tags { get; set; }
        public string? CustomFields { get; set; }
        public List<GTask>? Tasks { get; set; }
        public List<User>? resources { get; set; }
    }
    public class GOwner
    {
        public string id { get; set; }
        public string name { get; set; }
        public string? email { get; set; }

    }
    public class GAssignee
    {
        public string? id { get; set; }
        public string? name { get; set; }
    }
}
