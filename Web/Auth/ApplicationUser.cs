using Microsoft.AspNetCore.Identity;

namespace Web.Auth
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string? UserFullName { get; set; }
        // Add any additional properties or methods if needed
        public ICollection<Agent> Agents { get; set; }

        public ICollection<Agency> agencies { get; set; }




        public ICollection<Comment> Comments { get; set; }

        public ICollection<ShortList>? shortLists { get; set; }








    }
}
