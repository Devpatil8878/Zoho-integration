using System.Reflection.Metadata;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using Zoho_integration.Models;

namespace Zoho_integration.Models
{
    public class PortalResponse
    {
        public string? login_id { get; set; }
        public List<Portal>? portals { get; set; }
    }
    public class DocumentResponse
    {
        [JsonPropertyName("documents")]
        public List<Document>? Documents { get; set; }
    }
    public class TokenResponse
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public int expires_in { get; set; }
        public string token_type { get; set; }
        public string api_domain { get; set; }
        public int refresh_token_expires_in { get; set; }
    }
}
