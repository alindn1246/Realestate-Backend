using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Web.Auth
{
    public class Incpections
    {
        [Key]
        public int BookingId { get; set; }

        
        public DateTime? AvailableDateTime { get; set; }

        public DateTime? ReservedDateTime { get; set; }

        [Required]
        public int PropertyId { get; set; }

        // Navigation property
        [ForeignKey("PropertyId")]
        [JsonIgnore]
        public Property? Property { get; set; }
    }
}
