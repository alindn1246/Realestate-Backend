namespace Web.Auth
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text.Json.Serialization;

    public class AgentImage
    {
        
        
            [Key]
            public int ImageId { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string? ImageName { get; set; }

        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        [NotMapped]
        public string? ImageSrc { get; set; }

        public int AgentId { get; set; }
        [ForeignKey("AgentId")]
        [JsonIgnore]
        public Agent? Agent { get; set; } // Add this property

          
        
    }
}
