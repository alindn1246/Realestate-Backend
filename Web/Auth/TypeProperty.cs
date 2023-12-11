using System.ComponentModel.DataAnnotations;

namespace Web.Auth
{
    public class TypeProperty
    {
        [Key]
        public int TypeId { get; set; }

        [Required]
        public string ? TypeName { get; set; }
    }
}
