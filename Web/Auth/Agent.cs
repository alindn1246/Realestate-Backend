using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Web.Auth
{
    public class Agent
    {
        [Key]
        public int AgentId { get; set; }

        
        public string ?AgentName { get; set; }

        // UserId to associate the Agent with a User
        public int UserId { get; set; }

        [JsonIgnore]
        public ApplicationUser? User { get; set; }

        public int AgencyId { get; set; }

        [JsonIgnore]

        public Agency?Agency{ get; set;}







    }
}
