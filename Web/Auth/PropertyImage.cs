namespace Web.Auth
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text.Json.Serialization;

    public class PropertyImage
    {
        
        
            [Key]
            public int ImageId { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string? ImageName { get; set; }

        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        [NotMapped]
        public string? ImageSrc { get; set; }

        public int PropertyId { get; set; }
        [ForeignKey("PropertyId")]
        [JsonIgnore]
        public Property? Property { get; set; } // Add this property

          
        
    }
}
