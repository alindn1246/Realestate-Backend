using System.ComponentModel.DataAnnotations;

namespace Web.Auth
{
    public class FeutureProperty
    {
        [Key]
        public int Id { get; set; }

        public int PropertyId { get; set; }
        public Property? Property { get; set; }

        public int FeatureId { get; set; }
        public Feature ?Feature { get; set; }
    }
}
