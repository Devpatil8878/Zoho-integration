using System.Reflection.Metadata;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using Zoho_integration.Models;

namespace Zoho_integration.Models
{


    public class PortalResponse
    {
        public string login_id { get; set; }
        public List<Portal> portals { get; set; }
    }

    public class DocumentResponse
    {
        [JsonPropertyName("documents")]
        public List<Document> Documents { get; set; }
    }
   
    public class TimelogContainer
    {
        [JsonPropertyName("timelogs")]
        public TasklogContainer Timelogs { get; set; }
    }

    public class TasklogContainer
    {
        [JsonPropertyName("tasklogs")]
        public List<Timelog> Tasklogs { get; set; }
    }


}
