namespace Web.Auth
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text.Json.Serialization;

    public class Property
    {
        [Key]
        public int PropertyId { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public int TypeId { get; set; }

        [Required]
        public int AgentId { get; set; }

        [Required]
        public string? OwnerName { get; set; }

        [Required]
        public string? Address { get; set; }

        [Required]
        public int Price { get; set; }

        [Required]
        public int NbOfBeds { get; set; }

        [Required]
        public int Size { get; set; }

        [Required]
        public double Longitude { get; set; }

        [Required]
        public double Latitude { get; set; }

        // Navigation properties
        [ForeignKey("TypeId")]
        [JsonIgnore]
        public TypeProperty? Type { get; set; }

        [ForeignKey("AgentId")]
        [JsonIgnore]
        public Agent? Agent { get; set; }

        [JsonIgnore]
        public List<PropertyImage>? Images { get; set; }



        [JsonIgnore]
        public ICollection<Feature>? features { get; set; }


       

        public int UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
