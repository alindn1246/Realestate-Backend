using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Web.Auth
{
    public class Comment
    {
        [Key]
        public int RatingId { get; set; }

        
        public int Value { get; set; }

        public string? Comments { get; set; }

       
        public int UserId { get; set; }

        [JsonIgnore]
        public ApplicationUser? User { get; set; }

        public int ?AgentId { get; set; }

        [JsonIgnore]
        public Agent? Agent { get; set; }



    }
}
