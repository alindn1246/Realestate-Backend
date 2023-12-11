namespace Web.Auth
{
   
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Text.Json.Serialization;

    public class Feature
    {
        [Key]
        public int FeatureId { get; set; }

        [Required]
        public string ?FeatureName { get; set; }

        [JsonIgnore] 
        public ICollection<Property>? props { get; set; }

       
    }
}
