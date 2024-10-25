using System.Text.Json.Serialization;
namespace Zoho_integration.Models
{
    public class TaskCommentsResponse
    {
        [JsonPropertyName("comments")]
        public List<Comment>? Comments { get; set; }
    }
    public class Comment
    {
        [JsonPropertyName("id_string")]
        public string Id { get; set; }

        [JsonPropertyName("added_person")]
        public string? Author { get; set; }

        [JsonPropertyName("content")]
        public string? Text { get; set; }

        [JsonPropertyName("created_time_format")]
        public string? Timestamp { get; set; }
    }
    public class GCommentAuthor
    {
        public string? id { get; set; }
        public string name { get; set; }
        public string? email { get; set; }
    }
    public class GComment
    {
        public string id { get; set; }
        public GCommentAuthor? author { get; set; }
        public string? text { get; set; }
        public string? timestamp { get; set; }
    }
    public class GComments
    {
        public List<GComment>? Comments { get; set; }
    }
}
