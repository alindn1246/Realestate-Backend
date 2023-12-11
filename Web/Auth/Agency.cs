using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Web.Auth
{
    public class Agency
    {
        [Key]
        public int AgencyId { get; set; }

        
        public string ?AgencyName { get; set; }

        // UserId to associate the Agent with a User
        public int UserId { get; set; }

        [JsonIgnore]
        public ApplicationUser? User { get; set; }

       





    }
}
