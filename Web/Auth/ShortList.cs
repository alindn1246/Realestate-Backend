using System.ComponentModel.DataAnnotations;

namespace Web.Auth
{
    public class ShortList
    {
        [Key]
        public int ShortListId { get; set; }

        public int PropertyId { get; set; }
        public Property? Property { get; set; }


        public int UserId { get; set; }
        public ApplicationUser User { get; set; }

    }
}
